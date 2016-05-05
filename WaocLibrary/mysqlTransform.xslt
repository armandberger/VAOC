<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

<xsl:output method="xml" indent="yes"/>

  <!-- nouvelle version PHP -->
  <xsl:template match="/pma_xml_export">
  <xsl:for-each select="database">
    <xsl:element name="{@name}">
      <xsl:for-each select="table">
        <xsl:element name="{@name}">
          <xsl:for-each select="column">
            <xsl:element name="{@name}">
              <xsl:value-of select="." />
            </xsl:element>
          </xsl:for-each>
        </xsl:element>
      </xsl:for-each>
    </xsl:element>
  </xsl:for-each>
</xsl:template>

  <!-- ancienne version PHP, on ne change rien -->
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>
