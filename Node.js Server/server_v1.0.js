var net = require('net');
var fs = require('fs');
var sockets = [];
var fsockets = [];
var server = net.createServer(function (socket) {
    sockets.push(socket);
    console.log('New Client Connected..!! ');
    socket.setEncoding('utf8');
    //console.log(socket);
    socket.on('data', function (data) {
        //fs.appendFileSync('temp.txt', data);
        //if(data.parse(data))
        for (var i = 0; i < sockets.length; i++) {
            if (sockets[i] == socket) continue;
            sockets[i].write(data);
            console.log(sockets.length + '   ' + data);
        }
    });
    socket.on('end', function () {
        console.log('Client Disconnected');
        var i = sockets.indexOf(socket);
        sockets.splice(i, 1);
    });
});
server.listen(1337);
//server.listen(8008);
console.log('\n\nServer Started...!! \nlistening on port number 1337 for chat messages');


var server1 = net.createServer(function (socket1) {
    var filename = '';
    var flag = false;
    var buf;
    fsockets.push(socket1);
    console.log('New Client Connected..!! ');
    flag = false;
    var buffer = new Buffer(0, 'binary');
    socket1.on('data', function (data) {
        if (data == 'End')
        {
            fs.writeFile(filename, buffer);
            console.log('File Received: ' + filename);
            buffer = new Buffer(0, 'binary');
            flag = false;
            console.log(fsockets.length);
            console.log(filename);
            for (var i = 0; i < fsockets.length; i++) {
                if (fsockets[i] == socket1) continue;
                fsockets[i].write(filename, 'binary');
            }
            fs.readFile(filename, 'binary', function (err, data) {
                for (var i = 0; i < fsockets.length; i++)
                {
                    if (fsockets[i] == socket1) continue;
                    fsockets[i].write(data, 'binary');
                    console.log(data.length);
                    fsockets[i].write('@End@', 'binary');
                    console.log('File read');
                    filename = '';
                    console.log(data.length);
                }
            });
        }
        else if (flag) {
            buffer = Buffer.concat([buffer, new Buffer(data, 'binary')]);
            //fs.appendFileSync(filename, data);
            console.log(data.length);
        }
        else {
            flag = true;
            filename = '' + data;
            console.log(filename);
        }
    });
    socket1.on('end', function () {
        console.log('Client Disconnected');
        var i = fsockets.indexOf(socket1);
        sockets.splice(i, 1);

    });
});
server1.listen(1339);
console.log('listening on port number 1339 for file transfer');


