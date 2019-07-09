# Enigma

Enigma is a chat program that supports end to end encryption for group chats.

## Structure

**EnigmaClientCli** - Commandline Enigma Client.

**EnigmaLib** - Common models used by server and Client & Client API.

**EnigmaServer** - The Enigma REST API Server.

**EnigmaTest** - Unit tests for server and lib.

## How to run

All of the code are written by C# using .Net Core 2.2. .Net Core has a cross platform runtime and you can download it on its official website.

#### Server

To start the server, just change your working directory into ```EnigmaServer``` and type ```dotnet run``` in the command line. The server will be listened at ```localhost:5000```. Database will be reset at each start. To avoid this behavior, add ```--environment=production``` parameter.

#### Client

To start client, you can either use ```dotnet run``` to start the client or directly compile the program and run the dll using the runtime. To start multiple clients, use different working direcotries for each instance.