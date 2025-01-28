@echo off 

REM Verifica se algum argumento foi passado
if "%~1"=="" (
    echo Por favor, forne�a o nome da rede como argumento.
    echo Exemplo: create_network.cmd nome_da_rede
    pause
    exit /b 1
)

REM Define a vari�vel NET_NAME com o primeiro argumento
set NET_NAME=%~1

REM Verifica se a rede j� existe
docker network inspect %NET_NAME% >nul 2>&1

REM Se o c�digo de erro anterior (%ERRORLEVEL%) for 0, a rede j� existe
if %ERRORLEVEL% equ 0 (
    echo A rede '%NET_NAME%' j� existe.
) else (
    REM Caso contr�rio, cria a nova rede com o driver 'bridge'
    echo Criando a rede '%NET_NAME%'...
    docker network create -d bridge %NET_NAME%

    REM Verifica se a rede foi criada com sucesso
    if %ERRORLEVEL% equ 0 (
        echo Rede '%NET_NAME%' criada com sucesso.
    ) else (
        echo Falha ao criar a rede '%NET_NAME%'.
    )
)

REM Pausa o terminal para que voc� possa ver a sa�da
pause
