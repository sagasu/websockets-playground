# websockets-playground

# nodejs simplest solution 

The most simple solution is in nodejs folder. There is a server that runs on nodejs, and client that runs in a browser. To run server just run `npm start` in server folder, to run client just open index.html in a browser. When you click `Send` button in a browser you will send a message to the server that will respond with message of it's own. You will be able to see the respond in a browser console. From a server point of view you will see message inside node output.  

# nodejs server using socket.io

In nodejs folder there is one more project `server-usingsocketio` this is a really simple implementation of a server using socketio. You can connect to it using `client-usingsocketio` client.

Notice that both:
* `nodejs/client`
* `nodejs/client-usingsocketio`

Are really similar, the socketio one has a reference inside html file to socket.io lib hosted on cdn, that is about it. You can always instead of adding socketio client lib like this add it as an npm project if you are running react or angular.  