using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace Prism.API
{
    public class PrismItem : Item
    {

        #region PLANS
        //Make mod items derived from PrismItem

        //Include static collections to iterate over the items ByName, ByID, ByMod, etc

        /*
            Just leave the item's fields inherited from vanilla for simplicity.
            A modded item could look something like this [ but this won't be a mod it'll be my dev sword ;) ]

            namespace Meowmarimod
            {
                public class Meowmariblade : PrismItem
                {
                    private float timeSinceLastProjectile = 0;
                    private readonly float projectileDelay = 1.0f;

                    private readonly int trueCatShootNum = 5;
                    private readonly int trueCatDamage = 100;
                    private readonly float trueCatKnockback = 10f;
                    private readonly float trueCatShootVelocity = 30;

                    private readonly int fakeCatShootNum = 1;
                    private readonly int fakeCatDamage = 10;
                    private readonly float fakeCatKnockback = 0f;
                    private readonly float fakeCatShootVelocity = 10;

                    private readonly float catShootSpacing = 16;
                    private readonly float catShootSpread = MathHelper.PiOver4 / 9; //5 degrees

                    public Meowmariblade() : PrismItem()
                    {
                        this.rare = 10;
				        this.useSound = 1;
				        this.name = "Meowmariblade";
				        this.useStyle = 1;
				        this.damage = 300;
				        this.useAnimation = 16;
				        this.useTime = 12;
				        this.width = 30;
				        this.height = 30;
				        this.scale = 1.1f;
				        this.knockBack = 6.5f;
				        this.melee = true;
				        this.value = Item.sellPrice(0, 20, 0, 0);
				        this.autoReuse = true;

                        //10 vanilla Meowmeres = 1 Meowmariblade. Probably gonna want to do recipes less strangely than this.
                        PrismRecipe.AddRecipe((PrismRecipe r) => 
                        {
                            r.ResultantItem = this;
                            r.Count = 1;
                            r.AddIngredients(PrismItem.ByName["Terraria.Meowmere"], 10);
                        });

                        
                    }

                    //Called every frame during which the player has the item selected.
                    //GameTime gt: The Main instance's GameTime component.
                    public override void OnUpdate(GameTime gt)
                    {                        
                        timeSinceLastProjectile += (float)gt.ElapsedGameTime.TotalSeconds;
                    }

                    //Shoot lots of cats...
                    private void shootLotsOfCats()
                    {
                        //Angle to shoot cats (toward mouse cursor).
                        float angle = (float)Math.Atan2(Main.MouseWorld.X - Main.player[this.owner].Center.X, Main.MouseWorld.Y - Main.player[this.owner].Center.Y);

                        //Distance from player's center to spawn projectiles. Starts a little bit from the player center so the cats don't appear inside him/her.
                        float distStart = Main.player[this.owner].height + 8;
                                  
                        //Filthy imposters!       
                        bool trueOwner = (Main.player[this.owner].name == "Meowmaritus");                       
                        int catShootNum = trueOwner ? trueCatShootNum : fakeCatShootNum;
                        int catDamage = trueOwner ? trueCatDamage : fakeCatDamage;
                        float catKnockback = trueOwner ? trueCatKnockback : fakeCatKnockback;
                        float catShootVelocity = trueOwner ? trueCatShootVelocity : fakeCatShootVelocity;

                        //Shoot each cat
                        for (int i = 0; i < catShootNum; i++)
                        {
                            //This cat's distance from the player's center with spacing between cats applied.
                            float catRadiusDist = distStart + (catShootSpacing * i);

                            //Angle for this cat to shoot from with the cat shoot spread applied.
                            float catAngle = MathHelper.WrapAngle((float)(angle + ((Main.rand.NextDouble() - 0.5) * catShootSpread)));

                            //Unit coord for cat angle. Wow Trigonometry!!!
                            float catAngleCoord = new Vector2((float)Math.Sin(catAngle), (float)Math.Cos(catAngle));

                            //Actual position for the cat to shoot from.
                            float catPos = Main.player[this.owner].Center + (catAngleCoord * catRadiusDist);

                            //Initial velocity of the cat.
                            float catVelocity = catShootVelocity * catAngleCoord;                                                        

                            //Shoot our cat!
                            Projectile.NewProjectile(catPos.X, catPos.Y, catVelocity.X, catVelocity.Y, this.shoot, catDamage, catKnockback, this.owner);
                        }
                    }

                    //Called right as the player uses the item. Return value: whether to allow the player to use the item (null = vanilla).
                    public override bool? OnUse()
                    {
                        //Check if it's been long enough since we last shot lots of cats (because balance[-ish])
                        if (timeSinceLastProjectile >= projectileDelay)
                        {
                            shootLotsOfCats();
                            timeSinceLastProjectile = 0;
                        }

                        return null; //Let vanilla Terraria handle whether or not you can use the item (for Curse debuff, etc.)
                    }
                }
            }

            
        */
        #endregion

        /// <summary>
        /// Called every frame during which the player is holding the item.
        /// </summary>
        /// <param name="gameTime">The Main instance's GameTime component.</param>
        public virtual void OnUpdate(GameTime gameTime) { }

        /// <summary>
        /// Called the instant this item's owner uses the item.
        /// </summary>
        /// <returns>Whether the player is allowed to use the item this time. Null = Vanilla.</returns>
        public virtual bool? OnUse() { return null; }

        /// <summary>
        /// Called the instant the player selects the item.
        /// </summary>
        public virtual void OnSelect() { }

        /// <summary>
        /// Called the instant the player deselects the item.
        /// </summary>
        public virtual void OnDeselect() { }

    }
}
