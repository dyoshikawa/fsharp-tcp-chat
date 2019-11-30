format:
	~/.dotnet/tools/fantomas ./**/*.fs

lint:
	~/.dotnet/tools/dotnet-fsharplint -f ./Client/Client.fsproj
	~/.dotnet/tools/dotnet-fsharplint -f ./server/server.fsproj
