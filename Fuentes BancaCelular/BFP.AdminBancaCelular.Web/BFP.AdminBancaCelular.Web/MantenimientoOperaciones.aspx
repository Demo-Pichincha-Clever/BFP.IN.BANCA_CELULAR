<%@ Page Language="C#" MasterPageFile="~/AdminBancaCelular.Master" AutoEventWireup="true" 
CodeBehind="MantenimientoOperaciones.aspx.cs" Inherits="BFP.AdminBancaCelular.Web.MantenimientoOperaciones" 
Title="Mantenimiento de Operaciones" EnableEventValidation = "false"%>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script language="javascript" type="text/javascript">
   
    function confirm_delete()
    {
        if (confirm("¿Está seguro de eliminar la operación seleccionada?")==true)
            return true;
        else
            return false;
    }
    
    function ValidaComandos(source, arguments)
    {
      var txtComando1 = document.getElementById('<%= this.txtComando1.ClientID %>').value;
      var txtComando2 = document.getElementById('<%= this.txtComando2.ClientID %>').value;
      var txtComando3 = document.getElementById('<%= this.txtComando3.ClientID %>').value;
     
      Comando1 = txtComando1.toString().replace(/\$|\,/g,'').trim();
      Comando2 = txtComando2.toString().replace(/\$|\,/g,'').trim();
      Comando3 = txtComando3.toString().replace(/\$|\,/g,'').trim();
        
        
      if (Comando1 == '' && Comando2 == '' && Comando3 == '')
      {
          arguments.IsValid = true;
          return;
      }
      else
      {
          if (Comando1 != '' && Comando2 != '')
          {
              if (Comando1 == Comando2)
              {
                source.errormessage = "Los comandos ingresados deben ser únicos";
                arguments.IsValid = false;
                return;
              }
          }    
          
          if(Comando2 != '' && Comando3 != '')
          {
              if (Comando2 == Comando3)
              {
                source.errormessage = "Los comandos ingresados deben ser únicos";
                arguments.IsValid = false;
                return;
              }
          }
          
          if (Comando3 != '' && Comando1 != '')
          {
              if (Comando3 == Comando1)
              {
                source.errormessage = "Los comandos ingresados deben ser únicos";
                arguments.IsValid = false;
                return;
              }
          }
          else
          {
            arguments.IsValid = true;     
          }
      }
    }
    
    function AceptaSoloLetras()
    {
        if ((event.keyCode<65 || event.keyCode>90) && (event.keyCode<97 || event.keyCode>122) && event.keyCode!=32 && event.keyCode!=209 && event.keyCode!=241 && event.keyCode!=45 && event.keyCode!=46 && event.keyCode!=164 && event.keyCode!=165 )
        {
            event.returnValue = false;
        }
    }
    
    function AceptaSoloNroEntero()
    {
        if ((event.keyCode>57 || event.keyCode<48) && event.keyCode!=13)
        {
            event.returnValue = false;
        }
    }


  </script>

    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td height="35px" class="PantallaTitulo">
                Mantenimiento de Operaciones
            </td>
        </tr>
        <tr style="height: 40px">
            <td align="left" class="PantallaBotones">
                <asp:UpdatePanel ID="udpBotonesMenu" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:ImageButton ID="imgNuevo" runat="server" ImageUrl="~/Imagenes/BotonNuevo.gif"
                            ToolTip="Registrar nueva operación" OnClick="imgNuevo_Click" />
                        <asp:ImageButton ID="imgEliminar" runat="server" ImageUrl="~/Imagenes/BotonBajar.gif"
                            ToolTip="Eliminar operación" OnClick="imgEliminar_Click" OnClientClick ="return confirm_delete();" Visible ="false"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr style="height: 5px">
            <td style="font-size: 1px; background-color: #faeeb0">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="PantallaSubTitulo">
                <asp:ValidationSummary ID="valSumary" runat="server" ShowMessageBox="True" 
                    ShowSummary="False" ValidationGroup="ValidaOperacion" />
             </td>
        </tr>
        <tr>
            <td class="PantallaSubTitulo">
                <asp:Label ID="Label1" runat="server" Text="Listado de Operaciones" CssClass="PantallaSubTitulo"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="PantallaDatos">
                <asp:Label ID="lblDescOperacion" runat="server" Text="" CssClass="Descripcion"></asp:Label>
            </td>
        </tr>
        <tr style="height: 5px">
            <td style="font-size: 1px; background-color: #faeeb0">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="PantallaGrilla" align="center">
                <asp:UpdatePanel ID="udpOperaciones" runat="server" UpdateMode ="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="gvOperaciones" runat="server" AutoGenerateColumns="False" Width="700px"
                            AllowPaging="True" BorderColor="#DEDFDE" PageSize="4" BackColor="White" BorderStyle="None"
                            BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Both" 
                            onpageindexchanging="gvOperaciones_PageIndexChanging">
                            <FooterStyle BackColor="#CCCC99" />
                            <PagerStyle HorizontalAlign="right" BackColor="#F7F7DE" ForeColor="Black" />
                            <RowStyle BackColor="#F7F7DE" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderStyle Width="20px" />
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkOperId" runat="server" AutoPostBack="true" OnCheckedChanged="chkOperId_CheckedChanged" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Operación">
                                    <HeaderStyle Width="60px" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblIdOperacion" runat="server" Text='<%# Bind("IdOperacion") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Descripción" DataField="DesOperacion">
                                    <HeaderStyle Width="230px" />
                                    <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Comandos" DataField="Comandos">
                                    <HeaderStyle Width="140px" />
                                    <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Fecha Creación">
                                    <HeaderStyle Width="100px" HorizontalAlign ="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblFechaCreacion" runat="server" Text='<%# Bind("FechaCreacion") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign ="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <FooterStyle BackColor="#CCCC99" />
                            <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Right" />
                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" Font-Names="Tahoma" Font-Size="9pt"
                                Height="10px" ForeColor="White" />
                            <AlternatingRowStyle BackColor="#F7F7DE" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr style="height: 5px">
            <td style="font-size: 1px; background-color: #faeeb0">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="PantallaSubTitulo">
                <asp:Label ID="Label2" runat="server" Text="Datos de Operación" CssClass="PantallaSubTitulo"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="PantallaDatos">
        <tr>
            <td valign ="top">
                <asp:UpdatePanel ID="udpDatosI" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table class="PantallaDatos">
                            <tr>
                                <td class="PantallaDatosTituloI">
                                    <asp:Label ID="lblIDOperacion" runat="server" Text="Código:"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtIDOperacion" runat="server" Width="50px" CssClass="Campo_Texto"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfValCodigo" runat="server" 
                                        ControlToValidate="txtIDOperacion" Display="Dynamic" ValidationGroup ="ValidaOperacion"
                                        ErrorMessage="Debe ingresar el codigo de la operación">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="PantallaDatosTituloI">
                                    <asp:Label ID="lblNombreOperacion" runat="server" Text="Nombre:"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNombreOperacion" runat="server" Width="280px" CssClass="Campo_Texto"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfValNombreOperacion" runat="server" 
                                        ControlToValidate="txtNombreOperacion" Display="Dynamic" ValidationGroup ="ValidaOperacion"
                                        ErrorMessage="Debe ingresar el nombre de la operación">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="PantallaDatosTituloI">
                                    <asp:Label ID="lblComandos" runat="server" Text="Comandos:"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtComando1" runat="server" Width="70px" CssClass="Campo_Texto"></asp:TextBox>
                                    <asp:TextBox ID="txtComando2" runat="server" Width="70px" CssClass="Campo_Texto"></asp:TextBox>
                                    <asp:TextBox ID="txtComando3" runat="server" Width="70px" CssClass="Campo_Texto"></asp:TextBox>
                                    <asp:CustomValidator ID="cvalComandos" runat="server" 
                                      ClientValidationFunction="ValidaComandos" Display="Dynamic" ErrorMessage="*" ValidationGroup ="ValidaOperacion"
                                       ValidateEmptyText ="true"></asp:CustomValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="PantallaDatosTituloI">
                                    <asp:Label ID="lblOmision" runat="server" Text="Habilita omisión parámetros:"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkOmision" runat="server" 
                                        ValidationGroup="ValidaOperacion" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td valign ="top">
                <asp:UpdatePanel ID="udpDatosD" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table class="PantallaDatos">
                            <tr>
                                <td class="PantallaDatosTituloD">
                                    <asp:Label ID="lblProgramaAS" runat="server" Text="Transacción AS400:" Width="110px"></asp:Label>
                                </td>
                                <td width="200px">
                                    <asp:TextBox ID="txtProgramaAS" runat="server" Width="80px" MaxLength="6" CssClass="Campo_Texto"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfValNombrePrograma" runat="server" 
                                        ControlToValidate="txtProgramaAS" Display="Dynamic" 
                                        ErrorMessage="Debe ingresar la transacción AS400" ValidationGroup="ValidaOperacion">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="PantallaDatosTituloD">
                                    <asp:Label ID="lblProcesoBatch" runat="server" Text="Proceso Batch:"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkProcesoBatch" runat="server" Enabled="False" />
                                </td>
                            </tr>
                            <tr>
                                <td class="PantallaDatosTituloD">
                                    <asp:Label ID="lblHabilitado" runat="server" Text="Habilitado:"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkHabilitado" runat="server" Checked="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="PantallaDatosTituloD">
                                </td>
                                <td>

                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr style="height: 5px">
            <td style="font-size: 1px; background-color: #faeeb0">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="PantallaSubTitulo" colspan="2">
                <asp:Label ID="lblDatosParametro" runat="server" Text="Datos de Parámetros" CssClass="PantallaSubTitulo"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Descripcion" colspan="2">
                <asp:Label ID="Label3" runat="server" Text="Seleccione los parámetros a considerar para la operación:"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="PantallaDatos">
        <tr style="height: 5px">
            <td style="font-size: 1px; background-color: #faeeb0">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="PantallaGrilla" align="center">
                <asp:UpdatePanel ID="udpParametros" runat="server" UpdateMode ="Conditional" >
                    <ContentTemplate>
                        <asp:GridView ID="gvParametros" runat="server" AutoGenerateColumns="False" AllowPaging="True"
                            BorderColor="#DEDFDE" PageSize="5" BackColor="White" BorderStyle="None" BorderWidth="1px"
                            CellPadding="4" ForeColor="Black" GridLines="Both" 
                            onpageindexchanging="gvParametros_PageIndexChanging">
                            <FooterStyle BackColor="#CCCC99" />
                            <PagerStyle HorizontalAlign="right" BackColor="#F7F7DE" ForeColor="Black" />
                            <RowStyle BackColor="#F7F7DE" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderStyle Width="20px" />
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkParId" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Parámetro">
                                    <HeaderStyle Width="80px" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblIdParametro" runat="server" Text='<%# Bind("IdParametro") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Descripción">
                                    <HeaderStyle Width="220px" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblDescripcion" runat="server" Text='<%# Bind("DescripcionParametro") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Tipo de dato">
                                    <HeaderStyle Width="120px" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblTipoDato" runat="server" Text='<%# Bind("TipoDato") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Longitud">
                                    <HeaderStyle Width="60px" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblLongitud" runat="server" Text='<%# Bind("Longitud") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Decimal">
                                    <HeaderStyle Width="60px" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblDecimal" runat="server" Text='<%# Bind("Decimal") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <FooterStyle BackColor="#CCCC99" />
                            <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Right" />
                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" Font-Names="Tahoma" Font-Size="9pt"
                                Height="10px" ForeColor="White" />
                            <AlternatingRowStyle BackColor="#F7F7DE" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr style="height: 15px">
            <td style="font-size: 1px; background-color: #faeeb0">
                &nbsp;
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="udpBotonera" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="PantallaDatos">
                <tr>
                    <td style="width: 500px; padding-right: 5px;" align="right">
                        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="Boton" OnClick="btnGuardar_Click" ValidationGroup ="ValidaOperacion"/>
                    </td>
                    <td style="width: 500px; padding-right: 5px;" align="left">
                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="Boton" OnClick="btnCancelar_Click" />
                    </td>
                </tr>
                <tr style="height: 15px">
                    <td style="font-size: 1px; background-color: #faeeb0">
                        &nbsp;
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    
</asp:Content>
