module Tests.Test

open Expecto

let tests =
  test "A simple test" {
    let subject = "Hello World"
    Expect.equal subject "Hello World" "The strings should equal"
  }