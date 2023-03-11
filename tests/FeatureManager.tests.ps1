Describe "FeatureFlagTests" {
    BeforeAll {
        $baseUri = "https://localhost:7171"
    }

    It "Gets message from injected service fm/injected" {
        $message = Invoke-RestMethod "$baseUri/fm/injected"
        $message | Should -Match "^This is from"
    }

    It "Gets all flags /fm " {
        $flags = Invoke-RestMethod "$baseUri/fm"
        $flags.Count | Should -BeGreaterThan 3
        ($flags | Where-Object key -like 'PLAIN.KEY*').Count | Should -Be 3
        ($flags | Where-Object key -like 'CNTXT.KEY*').Count | Should -Be 3
   }

    It "Gets all context /fm/context" {
        $flags = Invoke-RestMethod "$baseUri/fm/context"
        $flags.Count | Should -BeGreaterThan 4
        ($flags | Where-Object key -like 'Context CNTXT.KEY*').Count | Should -Be 4
        ($flags | Where-Object value -eq $true).Count | Should -BeGreaterThan 3
   }

    It "Gets all no-context /fm/no-context" {
        $flags = Invoke-RestMethod "$baseUri/fm/no-context"
        $flags.Count | Should -Be 4
        ($flags | Where-Object key -like 'No Context CNTXT.KEY*').Count | Should -Be 4
        ($flags | Where-Object value -eq $false).Count | Should -Be 3
   }

    It "Gated test /fm/gated/testing" {
        $message = Invoke-RestMethod "$baseUri/fm/gated/testing"
        $message | Should -Be "This is gated testing"
   }
}

