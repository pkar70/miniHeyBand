' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238


Imports Microsoft.Toolkit.Uwp.Connectivity
Imports Windows.Devices.Bluetooth
Imports Windows.Devices.Bluetooth.Advertisement
Imports Windows.Devices.Bluetooth.GenericAttributeProfile
Imports Windows.Devices.Enumeration
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class BindBand
    Inherits Page

    Private moDevWatcher As BluetoothLEAdvertisementWatcher ' DeviceWatcher
    Private oTimer As DispatcherTimer
    Private Shared bShowAll = False

    Private Async Sub uiPage_Loaded(sender As Object, e As RoutedEventArgs)
        App.moLista = New ObservableCollection(Of Urzadzenie)
        If Not Await App.IsNetBTavailable(True) Then Exit Sub
        App.msNoName = App.GetLangString("msgNoName")

        moDevWatcher = New BluetoothLEAdvertisementWatcher
        moDevWatcher.ScanningMode = 0   ' tylko czeka, 1: żąda wysłania adv

        AddHandler moDevWatcher.Received, AddressOf BTwatch_Received
        moDevWatcher.Start()
        ' o wersji background: https://docs.microsoft.com/en-us/windows/uwp/devices-sensors/ble-beacon

        oTimer = New DispatcherTimer()
        oTimer.Interval = New TimeSpan(0, 0, 1)
        AddHandler oTimer.Tick, AddressOf TimerTick
        oTimer.Start()
    End Sub

    Public Sub TimerTick()
        Dim bZmiany As Boolean = False
        For Each oItem As Urzadzenie In App.moLista
            oItem.iCntTillDeath -= 1
            If oItem.iCntTillDeath = 0 Then bZmiany = True
        Next
        If bZmiany Then fromDispatch()
    End Sub
    Private Sub uiItem_Tapped(sender As Object, e As TappedRoutedEventArgs)
        ' select do bindnięcia
        Dim oItem As Urzadzenie

        ' odznacz wszystkie
        For Each oItem In App.moLista
            oItem.isSelected = False
            oItem.KolorBg = "White"
        Next

        ' a aktualnie klikniete zaznacz
        oItem = TryCast(TryCast(sender, Grid).DataContext, Urzadzenie)
        oItem.isSelected = True
        oItem.KolorBg = "Yellow"
        fromDispatch()
    End Sub

    Private Sub uiShowHidden_Click(sender As Object, e As RoutedEventArgs) Handles uiShowHidden.Click
        If uiShowHidden.Content = "Only known" Then
            uiShowHidden.Content = "All"
            bShowAll = True
            uiShowHidden.Icon = New SymbolIcon(Symbol.UnFavorite)
        Else
            uiShowHidden.Content = "Only known" ' 
            uiShowHidden.Icon = New SymbolIcon(Symbol.SolidStar)
            bShowAll = False
        End If
        uiShowHidden.IsChecked = False
    End Sub

    Private Sub uiPage_Unloaded(sender As Object, e As RoutedEventArgs)
        oTimer?.Stop()
        If moDevWatcher Is Nothing Then Exit Sub
        If moDevWatcher.Status <> BluetoothLEAdvertisementWatcherStatus.Started Then Exit Sub
        moDevWatcher.Stop()
    End Sub

    <CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId:="from")>
    Public Async Sub fromDispatch()
        If bInside Then
            For i As Integer = 1 To 10
                Await Task.Delay(10)
                If Not bInside Then Exit For
            Next
            bInside = True
        End If
        ListaItems.ItemsSource = From c In App.moLista Where c.iCntTillDeath > 0 Distinct
        bInside = False
    End Sub
    Public Async Sub toDispatch()
        Await ListaItems.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, AddressOf fromDispatch)
    End Sub

    Public Shared Function IsKnownBand(sName As String) As Boolean
        ' If bShowAll Then Return True
        ' Dim sFILTER_PREFIX = New Array
        If sName Is Nothing Then Return False

        Dim sFILTER_PREFIX As String() = {"T80", "T90", "T91", "T93", "T95", "TW68", "S9", "C60", "C67", "C67", "C68,", "C80,", "C86,", "C88,", "wxb_w4"}

        For i As Integer = 1 To sFILTER_PREFIX.GetUpperBound(0)
            If sName.StartsWith(sFILTER_PREFIX(i), StringComparison.Ordinal) Then Return True
        Next

        Return False
    End Function

    <CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId:="System.Byte.ToString(System.String)")>
    Public Shared Function BTaddrLong2String(lAddr As ULong) As String
        Dim sTxt As String = ""
        For i As Integer = 1 To 6
            Dim iByte As Byte = lAddr And &HFF
            sTxt = iByte.ToString("X2") & ":" & sTxt
            lAddr = lAddr >> 8
        Next
        Return sTxt.Substring(0, sTxt.Length - 1)
    End Function

    Private Shared bInside = False

    Private Async Sub BTwatch_Received(sender As BluetoothLEAdvertisementWatcher, oArgs As BluetoothLEAdvertisementReceivedEventArgs)
        Debug.WriteLine("Received: address=" & oArgs.BluetoothAddress & ", RSSI=" & oArgs.RawSignalStrengthInDBm)

        If bInside Then
            For i As Integer = 1 To 10
                Await Task.Delay(10)
                If Not bInside Then Exit For
            Next
            bInside = True
        End If

        Dim oDev As BluetoothLEDevice
        Dim iRSSI As Short = oArgs.RawSignalStrengthInDBm
        Dim sNoName As String = App.msNoName

        For Each oItem As Urzadzenie In App.moLista
            If oItem.AdresLong = oArgs.BluetoothAddress Then
                Dim bZmiany As Boolean = False
                If oItem.RSSIcurrent <> iRSSI Then
                    oItem.RSSIcurrent = iRSSI
                    oItem.RSSI = iRSSI & " dbm"
                    oItem.RSSImax = Math.Max(oItem.RSSImax, iRSSI)
                    oItem.RSSImin = Math.Min(oItem.RSSImin, iRSSI)
                    bZmiany = True
                End If
                oItem.iCntTillDeath = 20    ' liczymy od nowa
                If oItem.Nazwa = sNoName Then
                    ' sprobuj dodac nazwe
                    oDev = Await BluetoothLEDevice.FromBluetoothAddressAsync(oArgs.BluetoothAddress)
                    If oDev?.Name IsNot Nothing AndAlso oDev.Name <> "" Then
                        bZmiany = True
                        oItem.Nazwa = oDev.Name
                        If Not IsKnownBand(oDev.Name) Then
                            If Not bShowAll Then oItem.iCntTillDeath = -1
                        End If

                    End If
                End If
                ' jesli zmiana (dodanie nazwy, zmiana RSSI), redraw
                If bZmiany Then toDispatch()
                Exit Sub
            End If
        Next

        ' nie ma na liscie, to nalezy go dodac

        Dim oNew As Urzadzenie = New Urzadzenie
        oNew.Nazwa = sNoName  ' na razie, dopoki nie znajdzie
        oNew.AdresLong = oArgs.BluetoothAddress
        oNew.Adres = BTaddrLong2String(oArgs.BluetoothAddress)
        If oNew.Adres = App.GetSettingsString("bindedToAddr") Then
            oNew.KolorBg = "Yellow"
        Else
            oNew.KolorBg = "White"
        End If

        oNew.RSSIcurrent = iRSSI
        oNew.RSSI = iRSSI & " dbm"
        oNew.RSSImax = iRSSI
        oNew.RSSImin = iRSSI

        oDev = Await BluetoothLEDevice.FromBluetoothAddressAsync(oArgs.BluetoothAddress)
        If oDev?.Name IsNot Nothing AndAlso oDev.Name <> "" Then
            oNew.Nazwa = oDev.Name
            If Not IsKnownBand(oNew.Nazwa) Then
                If Not bShowAll Then oNew.iCntTillDeath = -1
            End If
        End If
        App.moLista.Add(oNew)

        toDispatch()
        bInside = False
    End Sub

    Private Async Sub ShowBasicParams()

        Dim sTxt As String = "Name: " & App.moBLE.Name & vbCrLf

        'Dim oDevInfo = DeviceInformation.CreateFromIdAsync(oBTDev.DeviceId)
        'App.moBLEobs = New ObservableBluetoothLEDevice(oDevInfo)
        'Dim oSvc = App.moBLE.BluetoothLEDevice.GetGattServicesForUuidAsync

        Dim sSvc As String = "0000180A-0000-1000-8000-00805F9B34FB"

        sTxt = sTxt & "Hardware Revision: " & Await App.ReadBLEchars(sSvc, "2A27", True) & vbCrLf
        sTxt = sTxt & "Firmware Revision: " & Await App.ReadBLEchars(sSvc, "2A26", True) & vbCrLf
        sTxt = sTxt & "Serial number: " & Await App.ReadBLEchars(sSvc, "2A25", True) & vbCrLf

        App.AppendLog("connect", "connect", sTxt)
        App.DialogBox(sTxt)

    End Sub


    Private Async Sub uiBind_Click(sender As Object, e As RoutedEventArgs)
        For Each oItem As Urzadzenie In App.moLista
            If oItem.isSelected Then
                App.SetSettingsString("bindedToName", oItem.Nazwa)
                App.SetSettingsString("bindedToAddr", oItem.AdresLong)
                App.SetSettingsString("bindedToAddrDisplay", oItem.Adres)
                App.moBLE = Await BluetoothLEDevice.FromBluetoothAddressAsync(oItem.AdresLong)

                ShowBasicParams()

                Me.Frame.GoBack()
                Exit Sub
            End If
        Next
        App.DialogBox("None is selected!")

    End Sub
End Class
