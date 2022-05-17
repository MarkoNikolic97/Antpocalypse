<?php

	$connection = mysqli_connect('localhost:3307', 'root', 'root', 'magic_survival');

	// check connection state
	if (mysqli_connect_errno()) 
	{
		echo "1";
		exit();
	}

	$username = $_POST["username"];
	$password = $_POST["password"];

	// check name existance
	$namecheckquery = "SELECT username, salt, hash, level, experience, currency1, currency2 FROM accounts WHERE username='" . $username . "';";

	$namecheck = mysqli_query($connection, $namecheckquery) or die("2");

	if (mysqli_num_rows($namecheck) != 1)
	{
		echo "5";
		exit();
	}

	// get login info
	$existingInfo = mysqli_fetch_assoc($namecheck);
	$salt = $existingInfo["salt"];
	$hash = $existingInfo["hash"];

	$loginHash = crypt($password, $salt);
	if ($hash != $loginHash) 
	{
		echo "6";
		exit();
	}

	echo "0\t" . $existingInfo["level"] . "\t" . $existingInfo["experience"] . "\t" . $existingInfo["currency1"] . "\t" . $existingInfo["currency2"];




?>