# masstransit-demos

This repo contains demo projects to help me learn using MassTransit with RabbitMq in .NET 6.

## v0.1

MTPing sends: 

>```
>> MTPing.exe
>MTPing: About to send 1000 messages to queue
>MTPing: Sent 1000 pings in 3378ms, @296 msg/sec
>```

and MTPong consumes messages of type `PingMessage`:
>```
>> MTPong.exe
>MTPong ready. Listening on queue "test", press a key to exit
>Received msg with sequence number 0.
>Received msg with sequence number 1.
>. . . .
>Received msg with sequence number 990.
>Received msg with sequence number 999.
>Received msg with sequence number 993.
>Received msg with sequence number 994.
>```
