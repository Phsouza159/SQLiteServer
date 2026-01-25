const net = require('net');

const HOST = '127.0.0.1';
const PORT = 50123;

const client = new net.Socket();

let isConnect = false;
setInterval(() => {
    client.connect(PORT, HOST, () => {
        console.log('Conectado ao servidor TCP');
    
        const payload = JSON.stringify({
            tipo: 'PING',
            data: new Date().toISOString()
        });
                
        client.write(payload);
        console.log('Enviado:', payload);
    });
}, 1);
client.on('data', (data) => {
    console.log('Recebido:', data.toString());
});

client.on('error', (err) => {
    console.error('Erro:', err.message);
});

client.on('close', () => {
    console.log('Conexão fechada');
});