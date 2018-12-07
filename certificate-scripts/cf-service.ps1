$ErrorActionPreference = "Stop"

$apiUrl = "https://api.system.{PAS DOMAIN}"
$username = "{PAS Username}"
$password = Read-Host -Prompt 'Input your PAS password'
$orgName = "{My Org}"
$spaceName = "{My Space}"

$serviceName = "credhub"
$servicePlan = "default"
$instanceName = "wcfApp-certificate"

$certificatePassword = "{a certificate password}"
$certificateBase64 = "{A certificate pfx converted to base 64 string}"

$serviceTags = [string]::Format('{0},certificate-store',$instanceName) #comma delimited

$paramJSON = [string]::Format('{{\"certificate\":\"{0}\",\"password\":\"{1}\"}}',$certificateBase64,$certificatePassword)

cf logout
cf login -a $apiUrl -u $username -p $password -o $orgName -s $spaceName
cf create-service $serviceName $servicePlan $instanceName -c $paramJSON -t $serviceTags
cf logout