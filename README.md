# Mandrill

**.NET-based cloud-native microservice integrating a Discord Bot, allowing full communication and state synchronization between any distributed cloud application and Discord community servers.** Wrapping this integration into its own independent Docker container mitigates the risk of service failure impacting the entire web application and allows for replication and "zero" downtime. Some of the features this service can provide to a cloud web application include:

1. **Support a seamless web SignIn/SignUp** system for the user by fetching an authenticated Discord user, obtaining public profile data, and the roles and permissions of that user for a given Discord community server.

2. **Eventual consistency between the cloud application state and the Discord community servers** state by using asynchronous communication between microservices with a message broker (RabbitMQ). This includes consistency for guild member roles, display names, avatars, guild status, and much more.

3. **Support for community events managed on the cloud to happen on Discord** by creating a new hidden category in the Discord server that ONLY web users registered for the event will be able to see on Discord. This is achieved by creating an event template where the event managers can define as many channels and the types of channels they want to be created for this event, along with all its characteristics. This includes **creating/deleting text and voice channels on Discord**, as well as assigning visibility of the channels to the right members according to the event setup from the web application.

4. Support for **automatic member distribution across the different event voice channels**, following the web event's template for member distribution.

5. Automatically register attendance and track the duration of each member's participation in the event, along with other event data.

6. Assigning/revoking, creating/deleting Discord roles from the cloud application.

7. **Track news** from the official Star Citizen webpage and post messages with updates in a dedicated Discord channel with the author, link to the resource, and a short description of the news. Also, **track in real time the game server status** and notify when an outage occurs.

This list highlights some features this service can provide, which may be extended in the future as needed.

