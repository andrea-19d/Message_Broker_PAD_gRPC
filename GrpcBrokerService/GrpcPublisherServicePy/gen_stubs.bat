@echo off
setlocal
pushd "%~dp0"

REM Points to ...\GrpcBrokerService\GrpcCommon\Protos (relative to this .bat)
set "PROTO_ROOT=%~dp0..\GrpcCommon\Protos"

if not exist "%PROTO_ROOT%" (
  echo ERROR: PROTO_ROOT not found: "%PROTO_ROOT%"
  echo Contents of parent for debugging:
  dir "%~dp0.."
  popd
  exit /b 1
)

REM Where generated Python stubs go
if not exist grpc_agent mkdir grpc_agent
if not exist grpc_agent\__init__.py type NUL > grpc_agent\__init__.py

echo Using PROTO_ROOT: "%PROTO_ROOT%"
echo Generating stubs into: "%CD%\grpc_agent"
echo.

REM IMPORTANT: Inputs must be relative to -I path (just filenames)
python -m grpc_tools.protoc ^
  -I "%PROTO_ROOT%" ^
  --python_out=.\grpc_agent ^
  --grpc_python_out=.\grpc_agent ^
  "publish.proto" "subscribe.proto" "notify.proto"

if errorlevel 1 (
  echo.
  echo protoc failed. Quick checks:
  echo  - Do these files exist? "%PROTO_ROOT%\publish.proto" "%PROTO_ROOT%\subscribe.proto" "%PROTO_ROOT%\notify.proto"
  echo  - Is grpcio-tools installed in this Python env?
  echo      python -m pip install --only-binary=:all: grpcio grpcio-tools protobuf
  popd
  exit /b 1
)

echo.
echo Done.
popd
endlocal
