# Durability

NOTE: DUE TO TMODLOADER UPDATES, THIS MOD HAS BUGS WITH CERTAIN FEATURES. USE AT YOUR OWN RISK.


Adds durability limits to weapons and armor. Formula for computing durability (approx. # of uses):

* Sell value of = x, Avg hits per second = y:
    0.5 * ( (y/4 * x^1.54) / (5 + x) ) + 50
* Value of valueless items:
    (dmg * y/4 * 1000 + def * 1000) * rarity
* Increases 2x if item is a tool or armor.
* Repair items via. reforging, armor via. smithing.
* Items permanently degrade with repairs.
* Config file in Documents/My Games/Terraria/ModLoader/Mod Configs/

https://forums.terraria.org/index.php?threads/durability.52036/
