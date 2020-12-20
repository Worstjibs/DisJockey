const Discord = require("discord.js");
const { prefix, token } = require("./botsettings.json");
const client = new Discord.Client();
const axios = require("axios");

const baseUrl = "http://localhost:5000/api";

let loggedInUsers = [];

let getUserById = function(id) {
    var checkUser = null;

    loggedInUsers.forEach(user => {
        if (user.id == id) {
            checkUser = user;
        }
    });

    return checkUser;
}

client.once("ready", async () => {
    console.log("Bot is Ready.");
});

client.on("message", async message => {
    const content = message.content.split(" ");
    const cmd = content[0];

    let author = message.author;

    if (!cmd.startsWith(prefix)) return;

    if (cmd.slice(prefix.length) == "play") {
        console.log("Play Command Used");

        data = {
            url: content[1],
            user: {
                discordId: author.id
            }
        }

        await axios.post(baseUrl + "/track", data)
        .then(response => {
            if (response.status == 200) {
                console.log("Track Added Successfully");
            }
        });

        return;      
    }

    if (cmd.slice(prefix.length) == "register") {
        let username = content[1];

        if (!username) {
            console.log("Please enter a username to register");
            return;
        }

        data = {
            username: username,
            discordId: author.id
        }

        axios.post(baseUrl + "/account/register", data)
            .then(response => {
                if (response.status == 200) {
                    console.log("User Registered Successfully");
                }
            })
            .catch(error => {
                console.log(error);
            })  
    }
});

client.login(token);