<?php

$servername = "localhost:3306";
$username = "root";
$password = "asd123";
$dbname = "biff";

$nickname = $_POST["nickname"];

$conn = new mysqli($servername, $username, $password, $dbname);
if($conn->connect_error)
{
	die("connection failed : ". $conn->connect_error);
}
// select * from characterInfo where  NickName = "�����";
$sql = "select * from characterInfo where NickName = '".$nickname."'";
$result = $conn->query($sql);
if($result->num_rows > 0)
{
	echo "[";
	while($row = $result->fetch_assoc()) // record �ϳ� $row�� ����
	{
		echo "{'NickName': '".$row['NickName']."', 'ID': '".$row['userId']."'},'";
	}
	echo "]";
}

else{
	echo "No UserInformation";
}
$conn->close();
?>