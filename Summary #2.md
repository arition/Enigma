# Week 2 Report

## New fetures

1. Complete the encryption and decryption method.
2. Setup the server and database.
3. Add rest API for server, haven't tested yet.

## Summary Report

The encryption and decrption function can be found in EnigmaLib folder. Small changes or modifications may apply in future if needed. (We use RSA to encrypt the AES key and we use AES-256 to encrypt the message.)

EnigmaServer folder contains the database (user, group, message, etc.) and the server we will use for our chat program. New features are still being added constantly, will have more in future. Currently we implemented the base database model and REST API for client. Next step we will focus on auth process of the client.

EnigmaTest is the unit test project where we use for testing. (Encryption test, decryption test, etc.)

*EnigmaClient is still undergoing, we will mainly focus on this part in our next phase.

Our project will focus on the direction of security. Our main purpose is to build a secure chat program. (trying to reach perfect secrecy)
