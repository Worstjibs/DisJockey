# DisJockey

DisJockey is a web application and Discord Bot, intended to play and track music that is played on a Discord server. It is build using .NET 8 Web API, Angular 11 and the Bot uses Discord.NET and Lavalink4Net. It uses MassTransit for API to Bot communication, so they remain large decoupled. This allows the bot to be self hosted whilst the API can run publicly in the cloud.

I used the following repositories as a guide for my work so far, thanks to the authors of them:  
Eliza: https://github.com/Pheubel/Eliza  
StreamMusicBot: https://github.com/DraxCodes/StreamMusicBot  

# Setup

First, setup a google cloud application with access to the YouTube API, which is used to grab video details on track play.

You'll also need access to a LavaLink server (https://github.com/lavalink-devs/Lavalink). You can host one on your local machine using the latest jar files, or run it Docker Container.

I've included a skeleton `appsettings.json` file in the root of the repo, you'll need to populate each of the empty properties with corresponding values and put it in the API folder.

Next, create a discord application at `https://discord.com/developers/applications`. Copy the Client Id and Client Secret values in the corresponding properties in appsettings of the API project. For the bot, copy the bot token to your user secrets in the bot project in the format `"BotSettings:BotToken": "{TOKEN}".`, or appsettings in a similar way. Finally, you need to add `{Application Hostname}/signin-discord` to the redirect URLs for your Discord Application, which is to ensure Discord Auth is successful.

# Running

DisJockey relies on docker extensively to run the required services. You can launch the app using docker compose (`docker compose up`). By default, this will launch the API (which also hosts the Angular client), the Bot, a Sql Server instance and RabbitMq. If you'd prefer to run SQL server on the host, simply comment out the `mssql` service in `docker-compose.override.yml` and update the `DefaultConnection` connection string in `appsettings.json`.

To make changes to Angular, you'll need to run `ng build` inside the client folder, as the API needs to serve static HTML files for Discord Authentication to work. You can use the argument `--watch` to automatically build the Angular solution when you make changes to it. Alternatively, I've added an npm command to `package.json`, which you can run by calling `npm run build`. Make sure you run this before launching docker compose, although I've added a mount to `wwwroot` so any changes will be picked up automatically, even when the app is running.

# Contributing

The project is in a rebuild stage after extensive refactoring and decoupling of the bot from the API, so can be unstable. Please raise an issue if you have any questions, and I will try to get back to you.
