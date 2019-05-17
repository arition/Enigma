# Summary Week-1

In this week we discussed the basic structure of our program. Our target is to make a chat program that supports end to end encryption for group chats.

We plan to use RSA for key exchange, and AES for the encryption of the message.

For the architecture, we plan to use a server-client model. Server only stores the public key of each account, so server cannot access the messages.

This week we mainly focus on the the design of the project.

## Client communication graph

```
                        +-------------------+
                        |                   |
                        |   Client Alice    |
                        |                   |
                        |                   |
                        +-----+-------------+
                              | 1.Send RSA Public key
               +-------------------------------+
+---------------------------------------------------------------+
|              |                               |                |
|              |                               |                |
|     +--------v---------+          +----------v---------+      |
|     |                  |          |                    |      |
|     |   Client Bob     |          |    Client Chalice  |      |
|     |                  |          |                    |      |
|     |                  |          |                    |      |
|     +------------------+          +--------------------+      |
|                     In a Group                                |
|                                                               |
+---------------------------------------------------------------+







                         +-------------------+
                         |                   |
                         |   Client Alice    |
               +--------->                   +<----+
               |         |                   |     |
               |         +-------------------+     | 2.Send their RSA public Key back to Alice
               |                                   |
               |                                   |
 +---------------------------------------------------------------+
 |             |                                   |             |
 |             |                                   |             |
 |     +-------+----------+          +-------------+------+      |
 |     |                  |          |                    |      |
 |     |   Client Bob     |          |    Client Chalice  |      |
 |     |                  |          |                    |      |
 |     |                  |          |                    |      |
 |     +------------------+          +--------------------+      |
 |                     In a Group                                |
 |                                                               |
 +---------------------------------------------------------------+


                           +-------------------+
                           |                   |
                           |   Client Alice    |
                           | send message 'AAA'|
                           |                   |
                 +---------+-------------------+--------+
                 |                                      |
                 |                                      |
         +-------v--------+                   +---------v-------+
         |  Encrypt with  |                   |  Encrypt with   |
         |  Bob's public  |                   |  Chalices's     |   Encrypt AES key using public key,
         |  key           |                   |  public key     |   Encrypt content using AES key
         |                |                   |                 |
         +--+-------------+                   +------------+----+
            |                                              |
            |                                              |
            |                                              |
+-----------v------+                              +--------v-----------+
|                  |                              |                    |
|   Client Bob     |                              |    Client Chalice  |
|  Decrypt with    |                              |decrypt with private|
|  private key     |                              |        key         |
+------------------+                              +--------------------+

```

## Trello link

https://trello.com/b/MFIJVob1/ecs153-project