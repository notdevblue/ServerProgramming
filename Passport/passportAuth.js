require("dotenv").config();
const express = require("express");
var passport = require("passport");
var app = express();
const router = express.Router();

app.use(passport.initialize());
app.use(router);

const spotifyStrategy = require("passport-spotify").Strategy;

passport.serializeUser(function (user, done) {
   done(null, user);
});

passport.deserializeUser(function (obj, done) {
   done(null, obj);
});

passport.use(new spotifyStrategy({
   clientID: process.env.SPOTIFY_ID,
   clientSecret: process.env.SPOTIFY_SECRET,
   callbackURL: "http://localhost:48000/auth/spotify/callback"
}, async (accessToken, refreshToken, profile, done) => {
   console.log("profile: " + profile.id);
   return done(null, profile);
})
);


router.get("/auth/spotify", passport.authenticate("spotify"));
router.get("/auth/spotify/callback", passport.authenticate("spotify", { failureRedirect: "/login" }),
   (req, res) => {
      res.redirect("/login");
      console.log("Success");
   }
);

router.get("/login", (req, res) => {
   res.send("Success!");
});

app.listen(48000, () => {
   console.log("App listening on port: 48000");
});