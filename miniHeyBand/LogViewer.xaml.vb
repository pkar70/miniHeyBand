' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

Imports Windows.Storage
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class LogViewer
    Inherits Page


    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Dim asNames() As String = {"device", "steps", "sleep", "connect", "internal", "tetno"}
        For Each sName As String In asNames
            Dim oCBItem As ComboBoxItem = New ComboBoxItem
            oCBItem.Content = sName
            uiLogName.Items.Add(oCBItem)
        Next
        uiLogLevel.Value = App.GetSettingsInt("logLevel", 0)
    End Sub

    Private Async Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim sName As String
        Try
            sName = TryCast(uiLogName.SelectedItem, ComboBoxItem).Content
        Catch ex As Exception
            Exit Sub
        End Try

        If sName = "internal" Then
            sName = App.GetSettingsString("internalog", "jeszcze pusty")
            uiClear.IsEnabled = True
        Else
            Dim oFile As StorageFile = Await App.GetLogFile(sName)
            If oFile Is Nothing Then Exit Sub
            sName = Await FileIO.ReadTextAsync(oFile)
            uiClear.IsEnabled = False
        End If

        uiLogView.Text = sName
        ' uiScroll.ScrollToVerticalOffset(uiScroll.ScrollableHeight)
        ' https://stackoverflow.com/questions/11171456/best-way-to-scroll-to-end-of-scrollviewer
        uiScroll.ChangeView(0, Double.MaxValue, 1)
        App.ClipPut(sName)
    End Sub

    Private Sub uiClear_Click(sender As Object, e As RoutedEventArgs)
        App.SetSettingsString("internalog", "reset")
    End Sub

    Private Sub uiLogLevel_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles uiLogLevel.ValueChanged
        App.SetSettingsInt("logLevel", uiLogLevel.Value)
    End Sub
End Class
