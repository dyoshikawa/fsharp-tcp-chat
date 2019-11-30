format:
	~/.dotnet/tools/fantomas ./**/*.fs

lint:
	~/.dotnet/tools/dotnet-fsharplint -f ./**/*.fsproj
