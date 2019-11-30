open System.Collections.Concurrent
open System.Net
open System.Net.Sockets
open System.Text

let sendBufToClients (clients: BlockingCollection<Socket>) (own: Socket) (buf: byte []): unit =
    for c in clients do
        if c.Equals(own) then ()
        else c.Send(buf) |> ignore

[<EntryPoint>]
let main _argv =
    let endPoint = IPEndPoint(IPAddress.Any, 8080)
    let socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
    socket.Bind(endPoint)
    socket.Listen(10)

    printfn "チャットサーバ開始 (%s)" <| socket.LocalEndPoint.ToString()

    let clients = new BlockingCollection<Socket>()

    let rec launch() =
        let client = socket.Accept()
        clients.Add(client)

        let newClientMsg = sprintf "入室 %s" <| client.RemoteEndPoint.ToString()
        printfn "%s" newClientMsg
        sendBufToClients clients client <| Encoding.Default.GetBytes newClientMsg

        async {
            let rec read() =
                let buffer =
                    [| for _ in 1 .. 10 -> byte (0) |]

                let len = client.Receive(buffer)
                if len = 0 then
                    let exitClientMsg = sprintf "退室 %s\n" <| client.RemoteEndPoint.ToString()
                    printfn "%s" exitClientMsg
                    sendBufToClients clients client <| Encoding.Default.GetBytes exitClientMsg
                    client.Close()
                    ()
                else
                    printfn "%s" <| Encoding.Default.GetString(buffer, 0, len)
                    sendBufToClients clients client buffer
                    read()
            read()
        }
        |> Async.Start

        launch()

    launch() |> ignore

    0
