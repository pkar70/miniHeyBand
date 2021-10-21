' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class SetDeviceClock
    Inherits Page

    ' tu mozna dodac jeszcze ustawianie czasu, tarczy
    ' DisplayClockReq		12
    ' ustawianie tarczy

    Private Sub uiPage_Loaded(sender As Object, e As RoutedEventArgs)
        uiDzien.Date = Date.Now
        uiGodzina.Time = New TimeSpan(Date.Now.Hour, Date.Now.Minute, 0)
    End Sub

    Private Async Sub uiCurrTime_Click(sender As Object, e As RoutedEventArgs)
        Await App.BandSendTime(Date.Now)
    End Sub

    Private Async Sub uiSomeTime_Click(sender As Object, e As RoutedEventArgs)
        If Not uiDzien.Date.HasValue Then Exit Sub

        If uiDzien.Date.Value > Date.Now OrElse uiDzien.Date.Value.Year < 2018 Then
            If Not Await App.DialogBoxResYN("msgFutureDate") Then Exit Sub
        End If

        Dim oDate As Date
        With uiDzien.Date.Value
            oDate = New Date(.Year, .Month, .Day,
                               uiGodzina.Time.Hours, uiGodzina.Time.Minutes, uiGodzina.Time.Seconds)
        End With

        Await App.BandSendTime(oDate)
    End Sub
End Class
