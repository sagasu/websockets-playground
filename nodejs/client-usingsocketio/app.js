const socket = io('ws://localhost:8080');

socket.on('message', (message) => {
    console.log(message);
    const el = document.createElement('li');
    el.innerText = message;
    document.querySelector('ul').appendChild(el);
});

document.querySelector('button').onclick = () => {
    const message = document.querySelector('input').value;
    socket.emit(`Hello from client: ${message}`);
};

