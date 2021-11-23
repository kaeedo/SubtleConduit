module SubtleConduit.Pages.Settings

open System
open Sutil
open SubtleConduit.Elmish
open SubtleConduit.Types
open Tailwind
open Sutil.Attr
open Sutil.DOM

let SettingsPage (model: State) dispatch =
    //let updateProfile
    let view =
        let user = model.User.Value
        let token =  user.Token

        //let state, dispatch = Store.makeElmish (init model) update ignore ()
        let url = Store.make user.Image
        let username = Store.make user.Username
        let email = Store.make user.Email
        let bio = Store.make user.Bio
        let password = Store.make String.Empty

        Html.div [
            disposeOnUnmount [
                url
                username
                email
                bio
                password
            ]

            Attr.classes [
                tw.container
                tw.``mx-auto``
                tw.flex
                tw.``flex-col``
                tw.``items-center``
            ]
            Html.h1 [
                Attr.classes [
                    tw.``text-4xl``
                    tw.``mb-2.5``
                ]
                text "Your Settings"
            ]
            Html.form [
                Attr.classes [
                    tw.``w-96``
                ]
                Html.div [
                    Attr.classes [
                        tw.``mb-4``
                    ]
                    Html.input [
                        Attr.classes [
                            tw.``border-2``
                            tw.``border-solid``
                            tw.rounded
                            tw.``border-gray-200``
                            tw.``px-6``
                            tw.``py-3``
                            tw.``w-full``
                        ]
                        type' "text"
                        Bind.attr ("value", url)
                        Attr.placeholder "URL of profile picture"
                    ]
                ]
                Html.div [
                    Attr.classes [
                        tw.``mb-4``
                    ]
                    Html.input [
                        Attr.classes [
                            tw.``border-2``
                            tw.``border-solid``
                            tw.rounded
                            tw.``border-gray-200``
                            tw.``px-6``
                            tw.``py-3``
                            tw.``w-full``
                        ]
                        type' "text"
                        Bind.attr ("value", username)
                        Attr.placeholder "Username"
                    ]
                ]
                Html.div [
                    Attr.classes [
                        tw.``mb-4``
                    ]
                    Html.textarea [
                        Attr.classes [
                            tw.``border-2``
                            tw.``border-solid``
                            tw.rounded
                            tw.``border-gray-200``
                            tw.``px-6``
                            tw.``py-3``
                            tw.``w-full``
                        ]
                        Attr.rows 8
                        Bind.attr ("value", bio)
                        Attr.placeholder "Bio"
                    ]
                ]
                Html.div [
                    Attr.classes [
                        tw.``mb-4``
                    ]
                    Html.input [
                        Attr.classes [
                            tw.``border-2``
                            tw.``border-solid``
                            tw.rounded
                            tw.``border-gray-200``
                            tw.``px-6``
                            tw.``py-3``
                            tw.``w-full``
                        ]
                        type' "text"
                        Bind.attr ("value", email)
                        Attr.placeholder "E-mail"
                    ]
                ]
                Html.div [
                    Attr.classes [
                        tw.``mb-4``
                    ]
                    Html.input [
                        Attr.classes [
                            tw.``border-2``
                            tw.``border-solid``
                            tw.rounded
                            tw.``border-gray-200``
                            tw.``px-6``
                            tw.``py-3``
                            tw.``w-full``
                        ]
                        type' "text"
                        Bind.attr ("value", password)
                        Attr.placeholder "New Password"
                    ]
                ]
                Html.div [
                    Attr.classes [
                        tw.flex
                        tw.``justify-end``
                    ]
                    Html.button [
                        Attr.classes [
                            tw.flex
                            tw.``bg-conduit-green``
                            tw.``hover:bg-conduit-green-500``
                            tw.``text-white``
                            tw.rounded
                            tw.``px-6``
                            tw.``py-3``
                            tw.``text-xl``
                        ]
                        type' "submit"
                        text "Update Settings"
                        onClick
                            (fun _ ->
                                dispatch (
                                    UpdateUser { UpsertUser.Username = username.Value
                                                 Image = url.Value
                                                 Bio = bio.Value
                                                 Token = Some token
                                                 Email = email.Value
                                                 Password = password.Value }))
                            []
                    ]
                ]
                Html.hr [
                    Attr.classes [
                        tw.``my-4``
                    ]
                ]
                Html.div [
                    Attr.classes [
                        tw.flex
                    ]
                    Html.button [
                        Attr.classes [
                            tw.flex
                            tw.border
                            tw.``bg-white``
                            tw.``border-red-700``
                            tw.``hover:bg-red-700``
                            tw.``text-red-700``
                            tw.``hover:text-white``
                            tw.rounded
                            tw.``px-4``
                            tw.``py-2``
                            tw.``text-lg``
                        ]
                        onClick (fun _ -> dispatch Logout) []
                        text "Or click here to logout"
                    ]
                ]
            ]
        ]

    view
