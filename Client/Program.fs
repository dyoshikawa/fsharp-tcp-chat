open System.IO
open System.Net
open System.Net.Sockets
open System.Text

[<EntryPoint>]
let main _argv =
    printfn "接続先を入力して下さい"
    let inputIp = stdin.ReadLine()

    let socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

    let ip =
        match inputIp with
        | "" -> IPAddress.Loopback
        | _ -> IPAddress.Parse(inputIp)

    let endPoint = IPEndPoint(ip, 8080)

    printfn "接続開始"
    socket.Connect(endPoint)
    printfn "接続成功"
    printfn "チャット開始"

    async {
        let rec receive(): unit =
            let buf =
                [| for _ in 1 .. 10 -> byte (0) |]

            let len = socket.Receive(buf)
            printf "%s" <| Encoding.Default.GetString(buf)
            receive()
        receive()
    }
    |> Async.Start

    let rec sendText(): int =
        let inputText = stdin.ReadLine()

        let sendBuf =
            Encoding.Default.GetBytes(sprintf "%s: %s\r\n" (socket.LocalEndPoint.ToString()) inputText)

        let sendResult = socket.Send(sendBuf)
        if sendResult = -1 then
            printfn "送信失敗"
            socket.Close()
            1
        else
            sendText()
    sendText()
