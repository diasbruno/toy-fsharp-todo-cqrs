DOTNET=$(shell which dotnet)

format:
	dotnet fantomas .

build: format
	dotnet build

run:
	dotnet run

clean:
	dotnet clean
