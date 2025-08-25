@REM
@REM Copyright (c) 2001-2018 Mathew A. Nelson and Robocode contributors
@REM All rights reserved. This program and the accompanying materials
@REM are made available under the terms of the Eclipse Public License v1.0
@REM which accompanies this distribution, and is available at
@REM http://robocode.sourceforge.net/license/epl-v10.html
@REM

java -Xmx512M -cp libs/robocode.jar;libs/jni4net.j-0.8.7.0.jar -XX:+IgnoreUnrecognizedVMOptions "--add-opens=java.base/sun.net.www.protocol.jar=ALL-UNNAMED" "--add-opens=java.base/java.lang.reflect=ALL-UNNAMED" "--add-opens=java.desktop/sun.awt=ALL-UNNAMED" robocode.Robocode %*
