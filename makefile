DOTNET=$(shell which dotnet)

build:
	dotnet build

format:
	dotnet fantomas .

run:
	dotnet run
