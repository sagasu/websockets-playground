const http = require('http').createServer();

const io = require('socket.io')(http, {
  cors: {origin: "*"}
});

io.on('connection', (socket) => {
  console.log('user connected');
  socket.on('message', (message) => {
    console.log(message);
    io.emit('message', `${socket.id.substr(0,2)} said ${message}`);
  });
  socket.on('disconnect', () => {
    console.log('user disconnected');
  });
  // socket.on('chat message', (msg) => {
  //   console.log(msg);
  //   io.emit('chat message', msg);
  // });
});

http.listen(8080, () => console.log('listening on http://localhost:8080'));