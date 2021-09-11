﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP.Utilities.NagadSetting
{
    public class SecurityKey : ISecurityKey
    {
        //Key for Live Nagad
        public static string marchentPrivateKey = "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCGtb4QP3hRllrnsfmJTy2cloYnTfXz1LzYnjUucjbaHMjPqXsYQuKLNSJJ84IyfjBFGKbvE+mKGgdw2PxFCdRIASlbe+z5e3EdT4rk65njInp5Z11mWlImLw9AJ5vneaRzxDq3PFtnEN1AAnlKOxBOGoOrMFfxjn6SKwK/FGoEXQc9SziBGxCETCBozn14xHprZ+rHnFJ3j/H+77lbnjoiYqpqif+KaR2j1gkYpGC5mLFxPolNzUh5MCqU6BHqqCHAV2Y6anvv+Oxf0hlJEffBrhN+NtGfbV+xFCKibWUu6/Fbxi0strpCxeI82CyFZt1G0qdJsyExghCb/atA/kRbAgMBAAECggEAeThGRhy3SsPAHcrbwCHN66DQK8JN1xLStSL4vEju7ysD0UqziEt3zMkNh/pmaMWA7kyWu4DxoOJ3W6cGq6GCsyDIdJh50K6yRSv00rboDapTB7hqJdaVIeMrGBIMiym9QKeOJqFbps1YK2eOeavYqk/VuFCScr5FS4cEu6nFFRK/wv8szOAPKQqqpyWVmNyti+FjjsMF0owSZyloAFgj9t94Qwf99e6ieYKWes6I1rsaivD3LemzeWl2cW6DJpnQeqYEyq5EAc4qrnAT6Bf26faXMXwepblCGGSE+wTFyloswZVgSz5vD4iMKGQFeUi0LgwmsF5N3lVNOj8wzS5OgQKBgQDxhkXZm+a7bDdas9ZQ69pWZ7oOmO5RhuaMDw0bnguD2rKNMR3qw9cOHcXuRE7i3Fgxlps/8Fq2jFeFnftM+SsG+qTyRsk5syC5SmQYhIzn3S2iKObM1kDrLSt339+O3WXizd3E4n2CF/JcsHxV77gIqJY4s4nLARMXeekijboluwKBgQCOyJsxeNNtE7t5A0T8vx7G0BrwyJi/hBZyML0xTldjVG01FGbk0VnKzEkKP/Utac0+PKPZsi2R3kYu5aBpEVi/zWv1jyIynmsekOYa/GV29FbnbDNk7b3kr39Z1sigc6SsN3rjGZFNtPs3Vq/nk/aSDTJU9KZYQul1MqkeSTIh4QKBgENYb22oHQQxvpWaA654tV7WeXjMh9dPyEVRhRQoU4mml5brFS57ASI8hw5TGbQEQMtC9XM0r/aq11S4onPqHmdJyGiN+FoQapV/+r0jaK7Wa30F7qU7MMriw3YfhJSvg1Nkl+voQ5rc1oWl6GWF/EjdyVVyKn/igtJO+gTahAc1AoGAY11n4z3apQYeaVDFKb83g9Q/a2GuK1mY5U7V2wJR/mY6ub3A/WxTuLYB/ogT7865bp50yIMfA2xUGz7iKzxPLuuePf2mJuPoWBUujYkoc2gHMnYyLgLlK8iWL8cxR7gy2Uai1nhsjk9spE5HjDmEVo+of0binHm0TkM7PkBiCwECgYEAgZPBjJ/hRuGso5JSagC76z2dwdQ6qHNtOdtrEvZo8yjyaPIs4X0jZW/32JFAe09UijZaAuFpKb8OGRlJ5LIj1v/mFI5UCcP7bJSshcmKrApTdMWJyixcDXF6zVGyQJYBPH21eFJNkuAZ6ZM5lx6KabmryIdZhdTJR1PfbQc2DYo="; //Get just the base64 content.

        public static string marchent = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAhrW+ED94UZZa57H5iU8tnJaGJ03189S82J41LnI22hzIz6l7GELiizUiSfOCMn4wRRim7xPpihoHcNj8RQnUSAEpW3vs+XtxHU+K5OuZ4yJ6eWddZlpSJi8PQCeb53mkc8Q6tzxbZxDdQAJ5SjsQThqDqzBX8Y5+kisCvxRqBF0HPUs4gRsQhEwgaM59eMR6a2fqx5xSd4/x/u+5W546ImKqaon/imkdo9YJGKRguZixcT6JTc1IeTAqlOgR6qghwFdmOmp77/jsX9IZSRH3wa4TfjbRn21fsRQiom1lLuvxW8YtLLa6QsXiPNgshWbdRtKnSbMhMYIQm/2rQP5EWwIDAQAB";

        public static string nagadPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAiCWvxDZZesS1g1lQfilVt8l3X5aMbXg5WOCYdG7q5C+Qevw0upm3tyYiKIwzXbqexnPNTHwRU7Ul7t8jP6nNVS/jLm35WFy6G9qRyXqMc1dHlwjpYwRNovLc12iTn1C5lCqIfiT+B/O/py1eIwNXgqQf39GDMJ3SesonowWioMJNXm3o80wscLMwjeezYGsyHcrnyYI2LnwfIMTSVN4T92Yy77SmE8xPydcdkgUaFxhK16qCGXMV3mF/VFx67LpZm8Sw3v135hxYX8wG1tCBKlL4psJF4+9vSy4W+8R5ieeqhrvRH+2MKLiKbDnewzKonFLbn2aKNrJefXYY7klaawIDAQAB";



        // Key for SandBox Test Nagad

        //public static string marchentPrivateKey = "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCJakyLqojWTDAVUdNJLvuXhROV+LXymqnukBrmiWwTYnJYm9r5cKHj1hYQRhU5eiy6NmFVJqJtwpxyyDSCWSoSmIQMoO2KjYyB5cDajRF45v1GmSeyiIn0hl55qM8ohJGjXQVPfXiqEB5c5REJ8Toy83gzGE3ApmLipoegnwMkewsTNDbe5xZdxN1qfKiRiCL720FtQfIwPDp9ZqbG2OQbdyZUB8I08irKJ0x/psM4SjXasglHBK5G1DX7BmwcB/PRbC0cHYy3pXDmLI8pZl1NehLzbav0Y4fP4MdnpQnfzZJdpaGVE0oI15lq+KZ0tbllNcS+/4MSwW+afvOw9bazAgMBAAECggEAIkenUsw3GKam9BqWh9I1p0Xmbeo+kYftznqai1pK4McVWW9//+wOJsU4edTR5KXK1KVOQKzDpnf/CU9SchYGPd9YScI3n/HR1HHZW2wHqM6O7na0hYA0UhDXLqhjDWuM3WEOOxdE67/bozbtujo4V4+PM8fjVaTsVDhQ60vfv9CnJJ7dLnhqcoovidOwZTHwG+pQtAwbX0ICgKSrc0elv8ZtfwlEvgIrtSiLAO1/CAf+uReUXyBCZhS4Xl7LroKZGiZ80/JE5mc67V/yImVKHBe0aZwgDHgtHh63/50/cAyuUfKyreAH0VLEwy54UCGramPQqYlIReMEbi6U4GC5AQKBgQDfDnHCH1rBvBWfkxPivl/yNKmENBkVikGWBwHNA3wVQ+xZ1Oqmjw3zuHY0xOH0GtK8l3Jy5dRL4DYlwB1qgd/Cxh0mmOv7/C3SviRk7W6FKqdpJLyaE/bqI9AmRCZBpX2PMje6Mm8QHp6+1QpPnN/SenOvoQg/WWYM1DNXUJsfMwKBgQCdtddE7A5IBvgZX2o9vTLZY/3KVuHgJm9dQNbfvtXw+IQfwssPqjrvoU6hPBWHbCZl6FCl2tRh/QfYR/N7H2PvRFfbbeWHw9+xwFP1pdgMug4cTAt4rkRJRLjEnZCNvSMVHrri+fAgpv296nOhwmY/qw5Smi9rMkRY6BoNCiEKgQKBgAaRnFQFLF0MNu7OHAXPaW/ukRdtmVeDDM9oQWtSMPNHXsx+crKY/+YvhnujWKwhphcbtqkfj5L0dWPDNpqOXJKV1wHt+vUexhKwus2mGF0flnKIPG2lLN5UU6rs0tuYDgyLhAyds5ub6zzfdUBG9Gh0ZrfDXETRUyoJjcGChC71AoGAfmSciL0SWQFU1qjUcXRvCzCK1h25WrYS7E6pppm/xia1ZOrtaLmKEEBbzvZjXqv7PhLoh3OQYJO0NM69QMCQi9JfAxnZKWx+m2tDHozyUIjQBDehve8UBRBRcCnDDwU015lQN9YNb23Fz+3VDB/LaF1D1kmBlUys3//r2OV0Q4ECgYBnpo6ZFmrHvV9IMIGjP7XIlVa1uiMCt41FVyINB9SJnamGGauW/pyENvEVh+ueuthSg37e/l0Xu0nm/XGqyKCqkAfBbL2Uj/j5FyDFrpF27PkANDo99CdqL5A4NQzZ69QRlCQ4wnNCq6GsYy2WEJyU2D+K8EBSQcwLsrI7QL7fvQ=="; //Get just the base64 content.

        //public static string marchentPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAiWpMi6qI1kwwFVHTSS77l4UTlfi18pqp7pAa5olsE2JyWJva+XCh49YWEEYVOXosujZhVSaibcKccsg0glkqEpiEDKDtio2MgeXA2o0ReOb9RpknsoiJ9IZeeajPKISRo10FT314qhAeXOURCfE6MvN4MxhNwKZi4qaHoJ8DJHsLEzQ23ucWXcTdanyokYgi+9tBbUHyMDw6fWamxtjkG3cmVAfCNPIqyidMf6bDOEo12rIJRwSuRtQ1+wZsHAfz0WwtHB2Mt6Vw5iyPKWZdTXoS822r9GOHz+DHZ6UJ382SXaWhlRNKCNeZavimdLW5ZTXEvv+DEsFvmn7zsPW2swIDAQAB";

        //public static string nagadPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAjBH1pFNSSRKPuMcNxmU5jZ1x8K9LPFM4XSu11m7uCfLUSE4SEjL30w3ockFvwAcuJffCUwtSpbjr34cSTD7EFG1Jqk9Gg0fQCKvPaU54jjMJoP2toR9fGmQV7y9fz31UVxSk97AqWZZLJBT2lmv76AgpVV0k0xtb/0VIv8pd/j6TIz9SFfsTQOugHkhyRzzhvZisiKzOAAWNX8RMpG+iqQi4p9W9VrmmiCfFDmLFnMrwhncnMsvlXB8QSJCq2irrx3HG0SJJCbS5+atz+E1iqO8QaPJ05snxv82Mf4NlZ4gZK0Pq/VvJ20lSkR+0nk+s/v3BgIyle78wjZP1vWLU4wIDAQAB";

        //Generate Random Number
        static Random r = new Random();
        public static int RandomNumber = r.Next(100000000, 999999999); //Randam Number should be less than 20 char


        //Initialize API URL NAGAD LIVE
        public static string InitializeAPI = "https://api.mynagad.com/api/dfs/check-out/initialize/";

        //Initialize API URL SANDBOX
       // public static string InitializeAPI = "http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/check-out/initialize/";
    }
}
