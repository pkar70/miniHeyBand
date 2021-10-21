' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class TimerOnOff
    Inherits Page

    Private Sub uiSet_Click(sender As Object, e As RoutedEventArgs)
        App.SetSettingsBool("autosaveSleep", uiSleep.IsOn)
        App.SetSettingsBool("autosaveSteps", uiSteps.IsOn)
    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        uiSteps.IsOn = App.GetSettingsBool("autosaveSteps")
        uiSleep.IsOn = App.GetSettingsBool("autosaveSleep")
    End Sub
End Class
