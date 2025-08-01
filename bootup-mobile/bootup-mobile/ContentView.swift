//
//  ContentView.swift
//  bootup-mobile
//
//  Created by Jacob Kelly on 7/29/25.
//

import SwiftUI

struct ContentView: View {
    @State private var isSending = false
    @State private var resultMessage: String?

    var body: some View {
        VStack(spacing: 20) {
            Button(action: {
                sendWakeRequest()
            }) {
                Text(isSending ? "Sending..." : "Wake PC")
                    .frame(maxWidth: .infinity)
                    .padding()
                    .background(isSending ? Color.gray : Color.blue)
                    .foregroundColor(.white)
                    .cornerRadius(10)
            }
            .disabled(isSending)

            if let message = resultMessage {
                Text(message)
                    .foregroundColor(.gray)
            }
        }
        .padding()
    }

    func sendWakeRequest() {
        guard let url = URL(string: "http://localhost:3000/wake") else {
            resultMessage = "Invalid URL"
            return
        }

        var request = URLRequest(url: url)
        request.httpMethod = "POST"

        isSending = true
        resultMessage = nil

        URLSession.shared.dataTask(with: request) { data, response, error in
            DispatchQueue.main.async {
                isSending = false

                if let error = error {
                    resultMessage = "Error: \(error.localizedDescription)"
                    return
                }

                if let httpResponse = response as? HTTPURLResponse {
                    if httpResponse.statusCode == 200 {
                        resultMessage = "Wake request sent successfully!"
                    } else {
                        resultMessage = "Failed with status: \(httpResponse.statusCode)"
                    }
                } else {
                    resultMessage = "Unexpected response"
                }
            }
        }.resume()
    }
}


#Preview {
    ContentView()
}
