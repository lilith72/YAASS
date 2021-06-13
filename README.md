# Yet Another Armor Set Search

## About

Yet Another Armor Set Search is a spiritual successor to Athena's ASS. This tool allows you to input the skills you want, and optionally your weapon slots and talismans, and will give you the combinations of armor which will let you have your dream build.

Current build: Alpha 0.1

## How to Use

Run YAASS.exe. This requires to be in the same folder it was packaged in, namely it needs to include the Newtonsoft DLL and the AppData folder.
Your data which is saved between runs (Talismans and Pinned sets) is stored in plaintext in your computer under %appdata%/YAASS/UserData.

## Configuration
In AppData/Config you will find a config.json file. You can edit this to adjust the behavior of the ASS.
- SearchMaxResults: Maximum number of results to find before the search stops working. Increasing this significantly may cause long times (on the order of minutes) to actually render the results, due to the way WPF works.
- SearchTimeoutSeconds: How long the ASS tries to search before giving up and returning the results it's already found. Increasing this will help find more results in complex searches, but you'll need to be patient as it will take longer.
- EnableLoggingToDisk: Whether to dump some informative logs to a file in the project folder. These logs will help if you want to report an issue.
- LogOutputFolder: Where logs should go.

## Known issues
- Search is currently inefficient and can take multiple minutes on complex searches.
- Armor skills don't appopriately account for bonuses from stormsoul level 4 or 5.

## Upcoming
YAASS is written and supported by three people with real life jobs and hobbies on a for-fun basis. No further updates are guaranteed, but realistically we'll probably update it for any new armors in Rise, and potentially future games. There are likely to be some engine optimizations to speed up search as well.

## Credits
YAASS is the product of three people's hard work.
- Justin, who wrote the search logic and some other core modules.
- TODO who designed and wrote the UI, going far above the minimum requirements to make it very polished.
- TODO who spent long hours gathering data for the engine and performed various testing.

Also:
- Special thanks to Athena. While we didn't leverage the existing code in athena's ASS and wrote it in a new framework, the concepts laid down in athena's ASS for world and previous MH games heavily inspired this tool.
- Netwonsoft.Json package is used under the MIT license https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md

## FAQs

- Why do I get more solutions when I add more skills to the search? A: The ASS doesn't try to fill your solutions with useless armors / decos. So if adding more skills makes more armors and decos "Useful", you might end up with more combinations. Generally you won't see this behavior for highly refined sets though.