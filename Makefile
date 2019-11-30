format:
	~/.dotnet/tools/fantomas ./Client/*.fs
	~/.dotnet/tools/fantomas ./Server/*.fs

lint:
	~/.dotnet/tools/dotnet-fsharplint -f ./Client/Client.fsproj
	~/.dotnet/tools/dotnet-fsharplint -f ./Server/Server.fsproj
