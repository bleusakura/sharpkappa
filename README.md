# Sharpkappa Twitch Bot
Twitch bot made in C# &amp; .NET Core with a focus on chat data analytics. 

## Current Build
  - Send bot to channel(s) through command line arguments when starting the bot
  - Logs every message sent by users in chat into seperate sqlite databases
  - Logs currently store (timestamp, username, message, channel, game being played, viewer count)
  - Independent static classes for interacting with Twich, FrankerfaceZ, & BetterTTV API's
  - Fetches emotes currently in use for each channel through said API's
  - Keeps track of emote usage by channel

## Usage
  - Follow your OS installation tips before usage
  - Inside src/App.config you will have to insert your bot account's oauth code, client-id, and client-secret
  - To generate the access token to be used with the API's, you can either use a third party or use the built in command for sharpkappa
  - To generate it through sharpkappa, run `dotnet run -g` after filling out the rest of the keys in App.config
  - Copy the output access token and paste it in App.config for the ACCESSTOKEN key
  - To run the bot, inside the src/ folder run `dotnet run channel(s)` Ex: `dotnet run forsen xqcow`
  - The databases will be generated in the paths specified

## Linux Installation
  - First install .NET 5.0 (https://docs.microsoft.com/en-us/dotnet/core/install/linux)
  - Clone this repository
  - Setup src/App.config as specified in Usage section above
  - Inside the /src directory run `dotnet run channel(s)` specificing each channel you want to the bot to be sent to with a space
  - The databases will be generated inside of the /src/data folder
 
 ## Windows 10 Installation
  - Install .NET 5.0 SDK (https://dotnet.microsoft.com/download)
  - Clone or download this repository
  - Setup src/App.config as specified in Usage section above
  - For windows your database path will have to be explicitly specified to the full directory path, from your drive letter to the folder. (C:\\...\\data)
  - Inside the /src directory run `dotnet run channel(s)` specificing each channel you want to the bot to be sent to with a space
  - The databases will be generated inside of the folder specified
  
  ## Next Steps For Project
  - Automatically generate statistics and graphs based on collected sqlite data
  - Create interactive website to display data
