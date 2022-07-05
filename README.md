# Corona Shooter
2D Unity game about shooting viruses. This game was created as a university project.

## Developers
#### Programmer
●	Justas Kajokas

#### Program Testers
●	Tadas Tkačenko\
● Aistis Kairys

#### Graphics Designers
●	Justas Kajokas\
●	Tadas Tkačenko\
● Aistis Kairys

#### Sound Designers
●	Tadas Tkačenko\
● Aistis Kairys

## Summary
In this game the player controls a needle moving through different levels and trying to eradicate viruses by shooting at them. Levels contain multiple waves. Each wave has increasing amount of viruses. At the end of every level there is a dedicated boss battle. For every killed virus the players gains discrete amount of in game currency, which can be used to buy various player upgrades.

## Controls
**W/A/S/D** - Movement\
**Space** - Shooting

## In-Game pictures
### Main Menu
<img width="1920" alt="CoronaShooterMainMenu" src="https://user-images.githubusercontent.com/100683761/177291258-d8d58728-c833-4306-bb27-cef3b1cb5f8e.png">

### Level Selector
<img width="1920" alt="CoronaShooterLevelSelector" src="https://user-images.githubusercontent.com/100683761/177294929-228da341-a584-4db3-9f0b-ab109da9f989.png">

### Upgrades
<img width="1920" alt="CoronaShooterUpgradesScreen" src="https://user-images.githubusercontent.com/100683761/177295439-0a7906f4-3da1-44d0-863b-54bffb8be059.png">

### Gameplay
<img width="1920" alt="CoronaShooterGameplay" src="https://user-images.githubusercontent.com/100683761/177295521-851dc62e-2d65-4e1b-8f44-eaf728c320d5.png">

## Programmer notes
One interesting aspect of this project is that it uses a new method of easily creating levels using XML files especially made for this project.

#### Example of XML file used for this project
    <?xml version="1.0" encoding="UTF-8"?>
    <levels>
      <level>
        <name>Level 1</name>
        <levelBackground>background_1.png</levelBackground>
        <sound>Level 1 Soundtrack</sound>
        <enemySprite>EnemySprite.png</enemySprite>
        <projectileSprite>Projectile1_0.anim</projectileSprite>
        <wave>
          <waveName>Wave: 1</waveName>
          <wavePic>wave_1_1.png</wavePic>
        </wave>
        <bossWave>
          <waveName>Boss wave</waveName>
          <wavePic>wave_1_boss.png</wavePic>
          <bossName>Level 1 Boss</bossName>
          <bossSound>Boss soundtrack</bossSound>
          <bossSprite>Boss1Sprite.png</bossSprite>
          <bossHealth>100</bossHealth>
          <bossCooldown>2</bossCooldown>
          <bossProjectileSpeed>5</bossProjectileSpeed>
          <bossProjectileSprite>Projectile2_0.anim</bossProjectileSprite>
        </bossWave>
      </level>
      <level>
        <name>Level 2</name>
        <levelBackground>background_2.png</levelBackground>
        <sound>Level 1 Soundtrack</sound>
        <enemySprite>EnemySprite.png</enemySprite>
        <projectileSprite>Projectile1_0.anim</projectileSprite>
        <wave>
          <waveName>Wave: 1</waveName>
          <wavePic>wave_2_1.png</wavePic>
        </wave>
        <bossWave>
          <waveName>Boss wave</waveName>
          <wavePic>wave_2_boss.png</wavePic>
          <bossName>Level 2 Boss</bossName>
          <bossSound>Boss soundtrack</bossSound>
          <bossSprite>BossSprite2.png</bossSprite>
          <bossHealth>300</bossHealth>
          <bossCooldown>3</bossCooldown>
          <bossProjectileSpeed>4</bossProjectileSpeed>
          <bossProjectileSprite>Projectile2_0.anim</bossProjectileSprite>
        </bossWave>
      </level>
      <level>
        <name>Level 3</name>
        <levelBackground>background_3.png</levelBackground>
        <sound>Level 1 Soundtrack</sound>
        <enemySprite>EnemySprite.png</enemySprite>
        <projectileSprite>Projectile1_0.anim</projectileSprite>
        <wave>
          <waveName>Wave: 1</waveName>
          <wavePic>wave_3_1.png</wavePic>
        </wave>
        <wave>
          <waveName>Wave: 2</waveName>
          <wavePic>wave_3_2.png</wavePic>
        </wave>
        <wave>
          <waveName>Wave: 3</waveName>
          <wavePic>wave_3_3.png</wavePic>
        </wave>
        <bossWave>
          <waveName>Boss wave</waveName>
          <wavePic>wave_3_boss.png</wavePic>
          <bossName>Level 3 Boss</bossName>
          <bossSound>Boss soundtrack</bossSound>
          <bossSprite>BossSprite3.png</bossSprite>
          <bossHealth>500</bossHealth>
          <bossCooldown>4</bossCooldown>
          <bossProjectileSpeed>5</bossProjectileSpeed>
          <bossProjectileSprite>Projectile2_0.anim</bossProjectileSprite>
        </bossWave>
      </level>
    </levels>

