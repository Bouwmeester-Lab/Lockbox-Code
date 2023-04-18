import serial
import sys
import os
import re

# chose an implementation, depending on os
#~ if sys.platform == 'cli':
#~ else:
if os.name == 'nt':  # sys.platform == 'win32':
    from serial.tools.list_ports_windows import comports
elif os.name == 'posix':
    from serial.tools.list_ports_posix import comports
#~ elif os.name == 'java':
else:
    raise ImportError("Sorry: no implementation for your platform ('{}') available".format(os.name))

class Port:
    def __init__(self, port, desc, hwid) -> None:
        self.port = port
        self.desc = desc
        self.hwid = hwid
    def __repr__(self) -> str:
        return f"{self.port:20}\n{self.desc} - HWID: {self.hwid}"

def select_port(key, dict):
    return dict[key]


def main():
    import argparse
    parser = argparse.ArgumentParser(description='Teensy multiple control software')

    parser.add_argument(
        '-s', '--include-links',
        action='store_true',
        help='include entries that are symlinks to real devices')
    
    parser.add_argument(
        '-n',
        type=int,
        help='only output the N-th entry')
    
    parser.add_argument(
        '-v', '--verbose',
        action='store_true',
        help='show more messages')

    parser.add_argument(
        '-q', '--quiet',
        action='store_true',
        help='suppress all messages')
    
    args = parser.parse_args()

    iterator = sorted(comports(include_links=args.include_links))

    ports = {}
    # list them
    for n, (port, desc, hwid) in enumerate(iterator, 1):
        if args.n is None or args.n == n:
            ports[n] = Port(port, desc, hwid)
            if args.verbose:
                sys.stdout.write("{:20}\n".format(port))
                sys.stdout.write("    desc: {}\n".format(desc))
                sys.stdout.write("    hwid: {}\n".format(hwid))

    for (key, port) in ports.items():
        print(f"[{key}] {port}")
    user_choice = input("Select a port number (between brackets)")
    try:
        k = int(user_choice)
        port = select_port(k, ports)
        print(f"Selected {port}")
    except:
        print("Exiting. Output is not an integer or port is not valid")
    


if __name__ == "__main__":
    main()
