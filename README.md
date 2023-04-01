# MandrilAPI
This project was created to satisfy the need of a service that provides communication and state synchronization between a guild official webpage and the official guilds discord server.
To achieve that I have choosen a micro-services design for the whole web application avoiding unnecessary bound between this service failure and the whole web application failure plus bringing easier CI/CD working flow, thanks to runnung this service in an independent docker container.
The features this service can provide to a guild web application are:

1-Sync in-webpage guild member's titles and roles with the guild's discord server roles. This includes creating, assigning/revoking and deleting discord roles from the webpage.

2-Support webpage events to happen on Discord by creating a new hiden category in the Discord server that ONLY web users registered in the event will be able to see on discord.This is achieved by creating an event template where the event managers can define as many channels and the type of chanels whey want to be created for this event and all its characteristics. This includes creating and deleting(at the and of the event) text and voice channles in discord. Also features moving users to voice channels and assigning visibility of the channels to the right members.

3-Support moving the members automatically when they text an ¨I'm ready¨ command to the Discord bot. Then the bot running on this service will move the ready member to the voice channel the event manager assigned for this person in thr webpage.

4-Register automatically assistance the time participation of each member in the event.

5-Track news from the official star citizen webpage and post messages with the updates in a dedicated discord channel with author, link to the resource and short description of the news.

6-Offer discord member verification by sending a verification code during the registration in thr webpage to verify the user is the owner of the provided discord handle during the registration.

The list of features this service can perform will be extended accordingly with the new requirements the web application may need in the future.
