FROM fsharp/fsharp

ADD . /app
WORKDIR /app

RUN /bin/bash build.sh
CMD mono bin/DummyApp/DummyApp.exe
