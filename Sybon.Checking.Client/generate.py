import subprocess, time, signal, os, sys
projectName = 'Sybon.Checking'
p = subprocess.Popen(["dotnet", "run"], cwd = os.path.join(os.getcwd(), projectName), stdin=subprocess.PIPE)
time.sleep(20)
subprocess.call(["java", "-jar", ".\\" + projectName + ".Client\\swagger-codegen-cli-3.0.0-20170904.171256-3.jar", "generate", "-l", "csharp", "--additional-properties", "targetFramework=v5.0,netCoreProjectFile=true,packageName="+projectName+".Client", "-i", "http://localhost:8194/swagger/v1/swagger.json", "-o", projectName+".Client\\"+projectName+".Client"])
p.send_signal(signal.CTRL_C_EVENT)
