<?php 
  $assetId = isset($_GET['assetId']) ? $_GET['assetId'] : false;
  $renderType = isset($_GET['renderType']) ? $_GET['renderType'] : false;
  $accessKey = isset($_GET['accessKey']) ? $_GET['accessKey'] : false;

  if (!$renderType || !$assetId || !$accessKey) {
    die("Missing parameters");
}

if ($accessKey !== "bloxrlcoolbloxrlv3rendereryep") {
    die("Invalid access key");
}

if (!in_array($renderType, ["Player", "Model", "Place", "Head", "Shirt", "Pants"])) {
    die("invalid render type set");
}

  file_get_contents($assetId, false, stream_context_create(['http' => ['ignore_errors' => true]]));

  header('Content-Type: text/plain; charset=utf-8'); 
  if(isset($_GET['signature']) && $_GET['signature'] == 'true') { ob_start(); } 
?>

game:GetService('Visit'):SetUploadUrl('')
for _,v in pairs(game.GuiRoot:GetChildren()) do v:Remove() end

<?php switch($renderType){ case "Player": ?> 
game.Lighting.TimeOfDay = '<?php echo random_int(7,12); ?>:<?php echo random_int(1,60); ?>:<?php echo random_int(1,60); ?>'
game:Load("rbxasset://greensky.rbxm")
if not game.Players:GetChildren()[1] then game.Players:CreateLocalPlayer(0) end
plr = game.Players.LocalPlayer 

plr.CharacterAppearance = "<?php echo $_GET['assetId']; ?>"
plr:LoadCharacter() 



wait(5)

for _,v in ipairs(plr.StarterGear:GetChildren()) do 
  if v.className == "Tool" then 
    plr.Character.Torso["Right Shoulder"].CurrentAngle = math.pi/2 
    plr.Character["Right Arm"].Transparency = 1
    plr.Character["Right Arm"].Transparency = 0
    v.Parent = plr.Character 
    for _,t in pairs(plr.Backpack:GetChildren()) do 
      t:Remove()
    end
    break 
  end 
end

wait(2)
<?php break; case "Model": ?>
game.Lighting.GeographicLatitude = 40
game.Lighting.TimeOfDay = '12:00:00'
game:Load("rbxasset://whitesky.rbxm")
local hatModel = game:GetObjects("<?php echo $_GET['assetId']; ?>")[1]
hatModel.Parent = workspace
error("hi")

<?php break; case "Place": ?>
game:Load("<?php echo $_GET['assetId']; ?>")
print("loaded <?php echo $_GET['assetId']; ?>")

for _,v in ipairs(game.Lighting:GetChildren()) do 
  if v.className == "Sky" then skybox = true end 
end
if not skybox then game:Load('rbxasset://sky.rbxm') end

for _,v in pairs(game.StarterPack:GetChildren()) do v:Remove() end

error("hi")

<?php break; case "Head"; ?>
game:Load("rbxasset://greensky.rbxm")
game.Players:CreateLocalPlayer(1)
game.Players.LocalPlayer:LoadCharacter()
wait(2)

local plr = game.Players.LocalPlayer.Character

local mesh = plr.Head:FindFirstChild("Mesh")
if mesh then mesh:Remove() end

if plr:FindFirstChild("Torso") then plr.Torso:Remove() end
if plr:FindFirstChild("Left Leg") then plr["Left Leg"]:Remove() end
if plr:FindFirstChild("Right Leg") then plr["Right Leg"]:Remove() end
if plr:FindFirstChild("Left Arm") then plr["Left Arm"]:Remove() end
if plr:FindFirstChild("Right Arm") then plr["Right Arm"]:Remove() end

local bab = game:GetObjects("<?php echo $_GET['assetId']; ?>")[1]

if bab then
    bab.Parent = plr.Head
end

plr.Head.BrickColor = BrickColor.new("Medium stone grey")

error("hi")
<?php break; case "Shirt": ?>

game.Lighting.TimeOfDay = '<?php echo random_int(7,12); ?>:<?php echo random_int(1,60); ?>:<?php echo random_int(1,60); ?>'
game:Load("rbxasset://greensky.rbxm")
if not game.Players:GetChildren()[1] then game.Players:CreateLocalPlayer(0) end
plr = game.Players.LocalPlayer 

plr.CharacterAppearance = "<?php echo $_GET['assetId']; ?>"
plr:LoadCharacter()

wait(3)

local char = plr.Character

char.Head.BrickColor = BrickColor.new("208")
char.Torso.BrickColor = BrickColor.new("208")
char["Left Arm"].BrickColor = BrickColor.new("208")
char["Right Arm"].BrickColor = BrickColor.new("208")
char["Left Leg"].BrickColor = BrickColor.new("208")
char["Right Leg"].BrickColor = BrickColor.new("208")

wait(2)
error("hi")

<?php break; case "Pants": ?>

game.Lighting.TimeOfDay = '<?php echo random_int(7,12); ?>:<?php echo random_int(1,60); ?>:<?php echo random_int(1,60); ?>'
game:Load("rbxasset://greensky.rbxm")
if not game.Players:GetChildren()[1] then game.Players:CreateLocalPlayer(0) end
plr = game.Players.LocalPlayer 

plr.CharacterAppearance = "<?php echo $_GET['assetId']; ?>"
plr:LoadCharacter()

wait(3)

local char = plr.Character

char.Head.BrickColor = BrickColor.new("208")
char.Torso.BrickColor = BrickColor.new("208")
char["Left Arm"].BrickColor = BrickColor.new("208")
char["Right Arm"].BrickColor = BrickColor.new("208")
char["Left Leg"].BrickColor = BrickColor.new("208")
char["Right Leg"].BrickColor = BrickColor.new("208")

wait(2)
error("hi")
<?php break; } 

  if(isset($_GET['signature']) && $_GET['signature'] == 'true') 
  { 
    $script = ob_get_clean();
    if(!filter_var($assetId, FILTER_VALIDATE_URL))
    { 
      $script = 'error("failed to verify render data URL")'; 
    }
    $privatekey = openssl_pkey_get_private("file://private_key.pem");
    openssl_sign($script, $signature, $privatekey);
    echo "%" . base64_encode($signature) . "%" . $script;
  }
?>
