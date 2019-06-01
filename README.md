# World of Auctions

Hello fellow goblins ! This is a console application, called World of Auctions, designed to help us make the most out of the data available out there about the auction house and its items.

## Pre-requisites

1. You need a Blizzard API Client. You can create one there : https://develop.battle.net/
2. You need a [tradeskillmaster](https://www.tradeskillmaster.com/) account

## Installation

1. Clone the repository
2. Add a secrets.config file inside src/WorldOfAuctions/ looking like this :

```xml
<?xml version="1.0" encoding="utf-8"?>
<appSettings>
  <add key="Blizzard_ClientId" value="{The ClientId of your blizzard client}" />
  <add key="Blizzard_ClientSecret" value="{The ClientSecret of your blizzard client" />
  <add key="TSM_ApiKey" value="{The TSM API Key located in your TSM 'My Account' page}" />
  <add key="DefaultRealm" value="{Your realm}" />
</appSettings>
```

By default the databse (SQLite) will be located in `C:\temp\woa.db`. If you want you can set it elsewhere by adding `<add key="DatabasePath" value="{elsewhere}" />` in secrets.config

3. Profit !

## How this works

### The APIs

#### TSM
The application will query TradeSkillMaster API with your given API KEY to get all items data for your realm and store it inside the database.
This can happen at most once per hour for each realm. Keep in mind the TSM API is limited to 25 of these calls per day, so if you have many realms and switch between them multiple times during the day, it might end up limited by quota.
There is room for improvement on the handling of the TSM API quotas.

#### Blizzard
The application will query Blizzard API for auctions every time you load it up or every time you change realm (with the `chrealm {realm}` command in the app).
Blizzard allows for 36,000 calls per hour to this API so this should be ok.

### The code

#### MediatR
The application's code uses MediatR to define notifications (the commands + their data) and notification handlers (the classes that do the work based on the command's data).
To define a Notification, simply create a class that implements INotification.

#### Attribute-based parsing
The main command, called by the running code, is the `ParseCommand`. Its handler will scan the assembly for any class decorated with `[WoACommand]`. It then browses these classes to check which command has a `RegexToMatch` matching the user input. When a command is found matching the input, it's instantiated with the resulting `Match` object to allow it to fill its properties. Finally the command is published through the mediator.

#### Example

```csharp
[WoACommand(RegexToMatch = @"spy (?<sellerName>.+)", Description = "See all auctions and info for given seller")]
public class SpySellerCommand : INotification
{
    public string SellerName { get; set; }

    public SpySellerCommand(Match match)
    {
        SellerName = match.Groups["sellerName"].Value;
    }
}
```
