---
date: 2026-02-15T09:18:00Z
role: user
conversation_id: react-frontend-creation
context: WroclawTheatreTickets full-stack application
redacted: false
---

# User Request: Create React Frontend

Create frontend based on react framework using routes and teailwind css. UI sould support themes switching light/dark and auto switch to system's default.

The UI should utilize api endpoints that already introduced in project.

The home page should show all the available tickets to the next 30 days in two columns on the page, each ticket should show name, date, stage address, price if available and "buy" button (payment url is available in the database).

The left navigation menu: Should contains url to the pages - home, show all, favorites and profile.

The client should support login/logout. Sign up and login buttons should be placed at the bottom of the navigation menu. On signup email would be used for the username and hve strong password with password confirmation. Email shoud be unique. After login user will be able to add ticket to the favorites list (star at the top right corner of the ticket) and remove the thicket from the favorites. Also should be a link to show all favorite tickets in one page (lets call it favorites). Upon login the block with sign up and login should be replced with the logout block.

The Show All page should have fields for the available filters and the list of the all available tickets in the system.

If the logged in user is moderator or administrator than this user should have ability to exclude tickets from the lists. Administrator should have their own landing page instead of home.

If you will have additional questions you can ask for details.
