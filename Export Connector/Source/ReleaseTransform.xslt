<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
                xmlns:ds="http://schemas.microsoft.com/developer/msbuild/2003"
>
    <xsl:output method="xml" indent="yes" encoding="utf-8" omit-xml-declaration="yes"/>

    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>

    <xsl:template match="ds:SignAssembly">
        <xsl:element name="{name()}" namespace="{namespace-uri()}">false</xsl:element>
    </xsl:template>

    <xsl:template match="ds:AssemblyOriginatorKeyFile">
        <xsl:element name="{name()}" namespace="{namespace-uri()}"/>
    </xsl:template>

    <xsl:template match="ds:SccProjectName">
        <xsl:element name="{name()}" namespace="{namespace-uri()}"/>
    </xsl:template>

    <xsl:template match="ds:SccLocalPath">
        <xsl:element name="{name()}" namespace="{namespace-uri()}"/>
    </xsl:template>

    <xsl:template match="ds:SccProvider">
        <xsl:element name="{name()}" namespace="{namespace-uri()}"/>
    </xsl:template>

    <xsl:template match="ds:ItemGroup/ds:Compile[@Include='..\..\..\..\CommonAssemblyInfo.vb']"/>

    <xsl:template match="ds:ItemGroup/ds:None[@Include='..\..\..\..\AscentCapture.snk']"/>

    <xsl:template match="ds:ItemGroup/ds:None[@Include='KCEC-Text.snk']"/>

    <xsl:template match="ds:PostBuildEvent">
        <xsl:element name="{name()}" namespace="{namespace-uri()}"/>
    </xsl:template>

    <xsl:template match="ds:PropertyGroup[contains(@Condition,'Debug')]/ds:OutputPath">
        <xsl:element name="{name()}" namespace="{namespace-uri()}">bin\Debug</xsl:element>
    </xsl:template>

    <xsl:template match="ds:PropertyGroup[contains(@Condition,'Release')]/ds:OutputPath">
        <xsl:element name="{name()}" namespace="{namespace-uri()}">bin\Release</xsl:element>
    </xsl:template>
</xsl:stylesheet>
