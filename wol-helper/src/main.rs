use axum::{routing::get, routing::post, Router};
use tokio::net::{TcpListener, UdpSocket};
use serde::Deserialize;
use std::fs;

#[tokio::main]
async fn main() {
    let config = load_config("config.json");

    let app: Router = Router::new()
        .route("/", get(|| async { "Hello, World!" }))
        .route("/wake", post({
            let mac = config.mac_address.clone();
            let addr = config.broadcast_addr.clone();
            move || async move {
                if let Err(e) = send_wake_on_lan(&mac, &addr).await {
                    eprintln!("Failed to send WOL packet: {}", e);
                    return axum::http::StatusCode::INTERNAL_SERVER_ERROR;
                }
                axum::http::StatusCode::OK
            }
        }));

    let listener: TcpListener = TcpListener::bind("0.0.0.0:3000").await.unwrap();
    axum::serve(listener, app).await.unwrap();
}

async fn send_wake_on_lan(mac_addr: &str, broadcast_addr: &str) -> std::io::Result<()> {
    let packet: Vec<u8> = build_wol_packet(mac_addr);

    println!("Binding to broadcast address: {}", broadcast_addr);
    let sock: UdpSocket = UdpSocket::bind("0.0.0.0:0").await?;
    sock.set_broadcast(true)?;

    println!("Sending Wake-on-LAN packet...");
    let len: usize = sock.send_to(&packet, broadcast_addr).await?;
    println!("Sent {} bytes", len);

    Ok(())
}

fn build_wol_packet(mac_addr: &str) -> Vec<u8> {
    println!("Building Wake-on-LAN packet for MAC address: {}", mac_addr);
    let mut packet: Vec<u8> = vec![0xFF; 6];
    let mac_bytes: Vec<u8> = mac_addr
        .split(':')
        .flat_map(|s| u8::from_str_radix(s, 16).ok())
        .collect::<Vec<u8>>();

    for _ in 0..16 {
        packet.extend_from_slice(&mac_bytes);
    }

    packet
}

#[derive(Debug, Deserialize)]
struct Config {
    mac_address: String,
    broadcast_addr: String,
}

fn load_config(path: &str) -> Config {
    let file_content = fs::read_to_string(path).expect("Failed to read config file");
    serde_json::from_str(&file_content).expect("Failed to parse config JSON")
}
