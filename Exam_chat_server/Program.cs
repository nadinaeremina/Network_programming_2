using Exam_chat_server;
using System.Net;
using System.Net.Sockets;

// создаем наш сервер
Server server = new Server();

// запускаем наш сервер
await server.ListenAsync(); 
