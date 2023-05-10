import serial
import sys
import os
import re
import constants



def select_port(key, dict):
    return dict[key]

def select_multiple_ports(ports):
    user_choice = ""
    while user_choice != "done":
        user_choice = input(constants.select_text).strip()
        try:
            select_port(int(user_choice), ports)
        except:
            print(constants.Errors.selection_error)


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
    
    
    try:
        k = int(user_choice)
        port = select_port(k, ports)
        print(f"Selected {port}")
    except:
        print("Exiting. Output is not an integer or port is not valid")
    


if __name__ == "__main__":
    main()
