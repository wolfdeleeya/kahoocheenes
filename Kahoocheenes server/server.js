const Websocket = require('ws');    //include ws

const PORT = 5000;
const LEN_OF_GAME_CODE = 4;

const wsServer = new Websocket.Server({
    port: PORT
});

class GameSocket {
    constructor(socket, num_of_players, code) {
        this.socket = socket;
        this.num_of_players = num_of_players;
        this.code = code;
    }
}

var game_sockets = new Map();

wsServer.on('connection', connection_callback);
console.log((new Date()) + " Server is listening on port " + PORT);

require('dns').lookup(require('os').hostname(), function (err, add, fam) {
    console.log('addr: ' + add + ":" + PORT + "randomly generated string is " + generate_code(4));
})

function connection_callback(socket) {
    console.log("A client just connected");
    socket.on('message', function (msg) {
        message_received_callback(socket, msg);
    });
}

function message_received_callback(socket, message) {
    if (message.length == 1 && message[0] == 0)                             //is it server code request message
        handle_game_connect(socket);
    else if (message.length == LEN_OF_GAME_CODE + 1 && message[0] == 0)     //is it server disconnect request message
        handle_game_disconnect(socket, message);
    else if (message.length == LEN_OF_GAME_CODE + 1 && message[0] == 1)     //is it controller connection request
        handle_controller_connect(socket, message);
    else if(message.length == LEN_OF_GAME_CODE + 3 && message[0] == 1)      //is it controller command
        handle_controller_command(socket, message);
}

function handle_game_connect(socket) {
    var result = generate_code(LEN_OF_GAME_CODE);
    while (game_sockets.has(result))
        result = generate_code(LEN_OF_GAME_CODE);
    var game_socket = new GameSocket(socket, 0, result);
    game_sockets.set(result, game_socket);
    socket.send('\0' + result);
}

function handle_game_disconnect(socket, message) {
    const code = message.toString().substring(1);
    game_sockets.delete(code);
}

function handle_controller_connect(socket, message) {
    const code = message.substring(1);
    if (game_sockets.has(code)) {    //does game exist
        var game_socket = game_sockets.get(code);
        game_socket.num_of_players += 1;
        game_sockets.set(code, game_socket);

        game_socket.socket.send([0, game_socket.num_of_players]);       //report new player connected to the game
        socket.send([1, game_socket.num_of_players]);                   //report player id to the player
    } else {    //game doesn't exist
        socket.send([1, 0]);    //report to the controller that sent code doesn't exist
    }
}

function handle_controller_command(socket, message) {
    const client_code = message[1];
    const command_code = message[2];
    const game_code = message.toString().substring(3);

    var game_socket = game_sockets.get(game_code);
    game_socket.socket.send([0, client_code, command_code]);    //send command to server
}

function get_random_int(max) {
    return Math.floor(Math.random() * max);
}

function generate_code(len_of_code) {
    var result = '';
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    const num_of_characters = characters.length;
    for (let i = 0; i < len_of_code; ++i)
        result += characters.charAt(get_random_int(num_of_characters));
    return result;
}