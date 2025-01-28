@echo off 

REM Verifica se algum argumento foi passado
if "%~1"=="" (
    echo Por favor, forneça o nome da rede como argumento.
    echo Exemplo: create_network.cmd nome_da_rede
    pause
    exit /b 1
)

REM Define a variável NET_NAME com o primeiro argumento
set NET_NAME=%~1

REM Verifica se a rede já existe
docker network inspect %NET_NAME% >nul 2>&1

REM Se o código de erro anterior (%ERRORLEVEL%) for 0, a rede já existe
if %ERRORLEVEL% equ 0 (
    echo A rede '%NET_NAME%' já existe.
) else (
    REM Caso contrário, cria a nova rede com o driver 'bridge'
    echo Criando a rede '%NET_NAME%'...
    docker network create -d bridge %NET_NAME%

    REM Verifica se a rede foi criada com sucesso
    if %ERRORLEVEL% equ 0 (
        echo Rede '%NET_NAME%' criada com sucesso.
    ) else (
        echo Falha ao criar a rede '%NET_NAME%'.
    )
)

REM Pausa o terminal para que você possa ver a saída
pause
