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

        var trackUser = getUserById(author.id);

        if (!trackUser) {
            console.log("User not logged in");
            return;
        }

        data = {
            url: content[1],
            user: {
                username: trackUser.username
            }
        }

        axios.post(baseUrl + "/track", data)
            .then(response => {
                if (response.status == 200) {
                    console.log("Track Added Successfully");
                }
            })
            .catch(error => {
                console.log(error);
            })  

        return;      
    }

    if (cmd.slice(prefix.length) == "setuser") {
        let username = content[1];

        let checkUser = getUserById(author.id);

        if (!checkUser) {
            checkUser = {
                id: author.id,
                username: username
            }
            loggedInUsers.push(checkUser);
        }

        console.log(checkUser);
        return;
    }
});

client.login(token);