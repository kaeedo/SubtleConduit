module SubtleConduit.Pages.Settings

open System
open Sutil
open SubtleConduit.Elmish
open SubtleConduit.Types
open Sutil.Attr
open Sutil.DOM

let SettingsPage (model: State) dispatch =
    //let updateProfile
    let view =
        let user = model.User.Value
        let token = user.Token

        //let state, dispatch = Store.makeElmish (init model) update ignore ()
        let url = Store.make user.Image
        let username = Store.make user.Username
        let email = Store.make user.Email
        let bio = Store.make user.Bio
        let password = Store.make String.Empty

        Html.div [
            disposeOnUnmount [ url; username; email; bio; password ]

            Attr.classes [
                "container"
                "mx-auto"
                "flex"
                "flex-col"
                "items-center"
            ]
            Html.h1 [
                Attr.classes [ "text-4xl"; "mb-2.5" ]
                text "Your Settings"
            ]
            Html.form [
                Attr.classes [ "w-96" ]
                Html.div [
                    Attr.classes [ "mb-4" ]
                    Html.input [
                        Attr.classes [
                            "border-2"
                            "border-solid"
                            "rounded"
                            "border-gray-200"
                            "px-6"
                            "py-3"
                            "w-full"
                        ]
                        type' "text"
                        Bind.attr ("value", url)
                        Attr.placeholder "URL of profile picture"
                    ]
                ]
                Html.div [
                    Attr.classes [ "mb-4" ]
                    Html.input [
                        Attr.classes [
                            "border-2"
                            "border-solid"
                            "rounded"
                            "border-gray-200"
                            "px-6"
                            "py-3"
                            "w-full"
                        ]
                        type' "text"
                        Bind.attr ("value", username)
                        Attr.placeholder "Username"
                    ]
                ]
                Html.div [
                    Attr.classes [ "mb-4" ]
                    Html.textarea [
                        Attr.classes [
                            "border-2"
                            "border-solid"
                            "rounded"
                            "border-gray-200"
                            "px-6"
                            "py-3"
                            "w-full"
                        ]
                        Attr.rows 8
                        Bind.attr ("value", bio)
                        Attr.placeholder "Bio"
                    ]
                ]
                Html.div [
                    Attr.classes [ "mb-4" ]
                    Html.input [
                        Attr.classes [
                            "border-2"
                            "border-solid"
                            "rounded"
                            "border-gray-200"
                            "px-6"
                            "py-3"
                            "w-full"
                        ]
                        type' "text"
                        Bind.attr ("value", email)
                        Attr.placeholder "E-mail"
                    ]
                ]
                Html.div [
                    Attr.classes [ "mb-4" ]
                    Html.input [
                        Attr.classes [
                            "border-2"
                            "border-solid"
                            "rounded"
                            "border-gray-200"
                            "px-6"
                            "py-3"
                            "w-full"
                        ]
                        type' "text"
                        Bind.attr ("value", password)
                        Attr.placeholder "New Password"
                    ]
                ]
                Html.div [
                    Attr.classes [ "flex"; "justify-end" ]
                    Html.button [
                        Attr.classes [
                            "flex"
                            "bg-conduit-green"
                            "hover:bg-conduit-green-500"
                            "text-white"
                            "rounded"
                            "px-6"
                            "py-3"
                            "text-xl"
                        ]
                        type' "submit"
                        text "Update Settings"
                        onClick
                            (fun _ ->
                                dispatch (
                                    UpdateUser
                                        {
                                            UpsertUser.Username = username.Value
                                            Image = url.Value
                                            Bio = bio.Value
                                            Token = Some token
                                            Email = email.Value
                                            Password = password.Value
                                        }
                                ))
                            []
                    ]
                ]
                Html.hr [ Attr.classes [ "my-4" ] ]
                Html.div [
                    Attr.classes [ "flex" ]
                    Html.button [
                        Attr.classes [
                            "flex"
                            "border"
                            "bg-white"
                            "border-red-700"
                            "hover:bg-red-700"
                            "text-red-700"
                            "hover:text-white"
                            "rounded"
                            "px-4"
                            "py-2"
                            "text-lg"
                        ]
                        onClick (fun _ -> dispatch Logout) []
                        text "Or click here to logout"
                    ]
                ]
            ]
        ]

    view
