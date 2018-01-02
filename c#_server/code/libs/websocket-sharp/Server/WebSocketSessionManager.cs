#region License
/*
 * WebSocketSessionManager.cs
 *
 * The MIT License
 *
 * Copyright (c) 2012-2015 sta.blockhead
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;

namespace WebSocketSharp.Server
{
  /// <summary>
  /// Manages the sessions in a Websocket service.
  /// </summary>
  public class WebSocketSessionManager
  {
    #region Private Fields

    private volatile bool                         _clean;
    private object                                _forSweep;
    private Logger                                _logger;
    private Dictionary<string, IWebSocketSession> _sessions;
    private volatile ServerState                  _state;
    private volatile bool                         _sweeping;
    private System.Timers.Timer                   _sweepTimer;
    private object                                _sync;
    private TimeSpan                              _waitTime;

    #endregion

    #region Internal Constructors

    internal WebSocketSessionManager ()
      : this (new Logger ())
    {
    }

    internal WebSocketSessionManager (Logger logger)
    {
      _logger = logger;

      _clean = true;
      _forSweep = new object ();
      _sessions = new Dictionary<string, IWebSocketSession> ();
      _state = ServerState.Ready;
      _sync = ((ICollection) _sessions).SyncRoot;
      _waitTime = TimeSpan.FromSeconds (1);

      setSweepTimer (60000);
    }

    #endregion

    #region Internal Properties

    internal ServerState State {
      get {
        return _state;
      }
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the IDs for the active sessions in the Websocket service.
    /// </summary>
    /// <value>
    /// An <c>IEnumerable&lt;string&gt;</c> instance that provides an enumerator which
    /// supports the iteration over the collection of the IDs for the active sessions.
    /// </value>
    public IEnumerable<string> ActiveIDs {
      get {
        foreach (var res in Broadping (WebSocketFrame.EmptyPingBytes, _waitTime))
          if (res.Value)
            yield return res.Key;
      }
    }

    /// <summary>
    /// Gets the number of the sessions in the Websocket service.
    /// </summary>
    /// <value>
    /// An <see cref="int"/> that represents the number of the sessions.
    /// </value>
    public int Count {
      get {
        lock (_sync)
          return _sessions.Count;
      }
    }

    /// <summary>
    /// Gets the IDs for the sessions in the Websocket service.
    /// </summary>
    /// <value>
    /// An <c>IEnumerable&lt;string&gt;</c> instance that provides an enumerator which
    /// supports the iteration over the collection of the IDs for the sessions.
    /// </value>
    public IEnumerable<string> IDs {
      get {
        if (_state == ServerState.ShuttingDown)
          return new string[0];

        lock (_sync)
          return _sessions.Keys.ToList ();
      }
    }

    /// <summary>
    /// Gets the IDs for the inactive sessions in the Websocket service.
    /// </summary>
    /// <value>
    /// An <c>IEnumerable&lt;string&gt;</c> instance that provides an enumerator which
    /// supports the iteration over the collection of the IDs for the inactive sessions.
    /// </value>
    public IEnumerable<string> InactiveIDs {
      get {
        foreach (var res in Broadping (WebSocketFrame.EmptyPingBytes, _waitTime))
          if (!res.Value)
            yield return res.Key;
      }
    }

    /// <summary>
    /// Gets the session instance with the specified <paramref name="id"/>.
    /// </summary>
    /// <value>
    ///   <para>
    ///   A <see cref="IWebSocketSession"/> instance or
    ///   <see langword="null"/> if not found.
    ///   </para>
    ///   <para>
    ///   That session instance provides the function to
    ///   access the information in the session.
    ///   </para>
    /// </value>
    /// <param name="id">
    /// A <see cref="string"/> that represents the ID of
    /// the session to find.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="id"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="id"/> is an empty string.
    /// </exception>
    public IWebSocketSession this[string id] {
      get {
        if (id == null)
          throw new ArgumentNullException ("id");

        if (id.Length == 0)
          throw new ArgumentException ("An empty string.", "id");

        IWebSocketSession session;
        tryGetSession (id, out session);

        return session;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the manager cleans up the inactive sessions in
    /// the WebSocket service periodically.
    /// </summary>
    /// <value>
    /// <c>true</c> if the manager cleans up the inactive sessions every 60 seconds;
    /// otherwise, <c>false</c>.
    /// </value>
    public bool KeepClean {
      get {
        return _clean;
      }

      internal set {
        if (!(value ^ _clean))
          return;

        _clean = value;
        if (_state == ServerState.Start)
          _sweepTimer.Enabled = value;
      }
    }

    /// <summary>
    /// Gets the sessions in the Websocket service.
    /// </summary>
    /// <value>
    /// An <c>IEnumerable&lt;IWebSocketSession&gt;</c> instance that provides an enumerator
    /// which supports the iteration over the collection of the sessions in the service.
    /// </value>
    public IEnumerable<IWebSocketSession> Sessions {
      get {
        if (_state == ServerState.ShuttingDown)
          return new IWebSocketSession[0];

        lock (_sync)
          return _sessions.Values.ToList ();
      }
    }

    /// <summary>
    /// Gets the wait time for the response to the WebSocket Ping or Close.
    /// </summary>
    /// <value>
    /// A <see cref="TimeSpan"/> that represents the wait time.
    /// </value>
    public TimeSpan WaitTime {
      get {
        return _waitTime;
      }

      internal set {
        if (value == _waitTime)
          return;

        _waitTime = value;
        foreach (var session in Sessions)
          session.Context.WebSocket.WaitTime = value;
      }
    }

    #endregion

    #region Private Methods

    private void broadcast (Opcode opcode, byte[] data, Action completed)
    {
      var cache = new Dictionary<CompressionMethod, byte[]> ();
      try {
        Broadcast (opcode, data, cache);
        if (completed != null)
          completed ();
      }
      catch (Exception ex) {
        _logger.Fatal (ex.ToString ());
      }
      finally {
        cache.Clear ();
      }
    }

    private void broadcast (Opcode opcode, Stream stream, Action completed)
    {
      var cache = new Dictionary <CompressionMethod, Stream> ();
      try {
        Broadcast (opcode, stream, cache);
        if (completed != null)
          completed ();
      }
      catch (Exception ex) {
        _logger.Fatal (ex.ToString ());
      }
      finally {
        foreach (var cached in cache.Values)
          cached.Dispose ();

        cache.Clear ();
      }
    }

    private void broadcastAsync (Opcode opcode, byte[] data, Action completed)
    {
      ThreadPool.QueueUserWorkItem (state => broadcast (opcode, data, completed));
    }

    private void broadcastAsync (Opcode opcode, Stream stream, Action completed)
    {
      ThreadPool.QueueUserWorkItem (state => broadcast (opcode, stream, completed));
    }

    private static string createID ()
    {
      return Guid.NewGuid ().ToString ("N");
    }

    private void setSweepTimer (double interval)
    {
      _sweepTimer = new System.Timers.Timer (interval);
      _sweepTimer.Elapsed += (sender, e) => Sweep ();
    }

    private void stop (PayloadData payloadData, bool send)
    {
      var bytes = send
                  ? WebSocketFrame.CreateCloseFrame (payloadData, false).ToArray ()
                  : null;

      lock (_sync) {
        _state = ServerState.ShuttingDown;

        _sweepTimer.Enabled = false;
        foreach (var session in _sessions.Values.ToList ())
          session.Context.WebSocket.Close (payloadData, bytes);

        _state = ServerState.Stop;
      }
    }

    private bool tryGetSession (string id, out IWebSocketSession session)
    {
      session = null;

      if (_state != ServerState.Start)
        return false;

      lock (_sync) {
        if (_state != ServerState.Start)
          return false;

        return _sessions.TryGetValue (id, out session);
      }
    }

    #endregion

    #region Internal Methods

    internal string Add (IWebSocketSession session)
    {
      lock (_sync) {
        if (_state != ServerState.Start)
          return null;

        var id = createID ();
        _sessions.Add (id, session);

        return id;
      }
    }

    internal void Broadcast (
      Opcode opcode, byte[] data, Dictionary<CompressionMethod, byte[]> cache)
    {
      foreach (var session in Sessions) {
        if (_state != ServerState.Start)
          break;

        session.Context.WebSocket.Send (opcode, data, cache);
      }
    }

    internal void Broadcast (
      Opcode opcode, Stream stream, Dictionary <CompressionMethod, Stream> cache)
    {
      foreach (var session in Sessions) {
        if (_state != ServerState.Start)
          break;

        session.Context.WebSocket.Send (opcode, stream, cache);
      }
    }

    internal Dictionary<string, bool> Broadping (byte[] frameAsBytes, TimeSpan timeout)
    {
      var ret = new Dictionary<string, bool> ();
      foreach (var session in Sessions) {
        if (_state != ServerState.Start)
          break;

        ret.Add (session.ID, session.Context.WebSocket.Ping (frameAsBytes, timeout));
      }

      return ret;
    }

    internal bool Remove (string id)
    {
      lock (_sync)
        return _sessions.Remove (id);
    }

    internal void Start ()
    {
      lock (_sync) {
        _sweepTimer.Enabled = _clean;
        _state = ServerState.Start;
      }
    }

    internal void Stop (ushort code, string reason)
    {
      if (code == 1005) { // == no status
        stop (PayloadData.Empty, true);
        return;
      }

      stop (new PayloadData (code, reason), !code.IsReserved ());
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Sends <paramref name="data"/> to every client in the WebSocket service.
    /// </summary>
    /// <param name="data">
    /// An array of <see cref="byte"/> that represents the binary data to send.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the manager is not Start.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="data"/> is <see langword="null"/>.
    /// </exception>
    public void Broadcast (byte[] data)
    {
      if (_state != ServerState.Start) {
        var msg = "The current state of the manager is not Start.";
        throw new InvalidOperationException (msg);
      }

      if (data == null)
        throw new ArgumentNullException ("data");

      if (data.LongLength <= WebSocket.FragmentLength)
        broadcast (Opcode.Binary, data, null);
      else
        broadcast (Opcode.Binary, new MemoryStream (data), null);
    }

    /// <summary>
    /// Sends <paramref name="data"/> to every client in the WebSocket service.
    /// </summary>
    /// <param name="data">
    /// A <see cref="string"/> that represents the text data to send.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the manager is not Start.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="data"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="data"/> could not be UTF-8-encoded.
    /// </exception>
    public void Broadcast (string data)
    {
      if (_state != ServerState.Start) {
        var msg = "The current state of the manager is not Start.";
        throw new InvalidOperationException (msg);
      }

      if (data == null)
        throw new ArgumentNullException ("data");

      byte[] bytes;
      if (!data.TryGetUTF8EncodedBytes (out bytes)) {
        var msg = "It could not be UTF-8-encoded.";
        throw new ArgumentException (msg, "data");
      }

      if (bytes.LongLength <= WebSocket.FragmentLength)
        broadcast (Opcode.Text, bytes, null);
      else
        broadcast (Opcode.Text, new MemoryStream (bytes), null);
    }

    /// <summary>
    /// Sends the data from <paramref name="stream"/> to every client in
    /// the WebSocket service.
    /// </summary>
    /// <remarks>
    /// The data is sent as the binary data.
    /// </remarks>
    /// <param name="stream">
    /// A <see cref="Stream"/> instance from which to read the data to send.
    /// </param>
    /// <param name="length">
    /// An <see cref="int"/> that specifies the number of bytes to send.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the manager is not Start.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="stream"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="stream"/> cannot be read.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="length"/> is less than 1.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   No data could be read from <paramref name="stream"/>.
    ///   </para>
    /// </exception>
    public void Broadcast (Stream stream, int length)
    {
      if (_state != ServerState.Start) {
        var msg = "The current state of the manager is not Start.";
        throw new InvalidOperationException (msg);
      }

      if (stream == null)
        throw new ArgumentNullException ("stream");

      if (!stream.CanRead) {
        var msg = "It cannot be read.";
        throw new ArgumentException (msg, "stream");
      }

      if (length < 1) {
        var msg = "Less than 1.";
        throw new ArgumentException (msg, "length");
      }

      var bytes = stream.ReadBytes (length);

      var len = bytes.Length;
      if (len == 0) {
        var msg = "No data could be read from it.";
        throw new ArgumentException (msg, "stream");
      }

      if (len < length) {
        _logger.Warn (
          String.Format (
            "Only {0} byte(s) of data could be read from the stream.",
            len
          )
        );
      }

      if (len <= WebSocket.FragmentLength)
        broadcast (Opcode.Binary, bytes, null);
      else
        broadcast (Opcode.Binary, new MemoryStream (bytes), null);
    }

    /// <summary>
    /// Sends <paramref name="data"/> asynchronously to every client in
    /// the WebSocket service.
    /// </summary>
    /// <remarks>
    /// This method does not wait for the send to be complete.
    /// </remarks>
    /// <param name="data">
    /// An array of <see cref="byte"/> that represents the binary data to send.
    /// </param>
    /// <param name="completed">
    ///   <para>
    ///   An <see cref="Action"/> delegate or <see langword="null"/>
    ///   if not needed.
    ///   </para>
    ///   <para>
    ///   The delegate invokes the method called when the send is complete.
    ///   </para>
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the manager is not Start.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="data"/> is <see langword="null"/>.
    /// </exception>
    public void BroadcastAsync (byte[] data, Action completed)
    {
      if (_state != ServerState.Start) {
        var msg = "The current state of the manager is not Start.";
        throw new InvalidOperationException (msg);
      }

      if (data == null)
        throw new ArgumentNullException ("data");

      if (data.LongLength <= WebSocket.FragmentLength)
        broadcastAsync (Opcode.Binary, data, completed);
      else
        broadcastAsync (Opcode.Binary, new MemoryStream (data), completed);
    }

    /// <summary>
    /// Sends <paramref name="data"/> asynchronously to every client in
    /// the WebSocket service.
    /// </summary>
    /// <remarks>
    /// This method does not wait for the send to be complete.
    /// </remarks>
    /// <param name="data">
    /// A <see cref="string"/> that represents the text data to send.
    /// </param>
    /// <param name="completed">
    ///   <para>
    ///   An <see cref="Action"/> delegate or <see langword="null"/>
    ///   if not needed.
    ///   </para>
    ///   <para>
    ///   The delegate invokes the method called when the send is complete.
    ///   </para>
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the manager is not Start.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="data"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="data"/> could not be UTF-8-encoded.
    /// </exception>
    public void BroadcastAsync (string data, Action completed)
    {
      if (_state != ServerState.Start) {
        var msg = "The current state of the manager is not Start.";
        throw new InvalidOperationException (msg);
      }

      if (data == null)
        throw new ArgumentNullException ("data");

      byte[] bytes;
      if (!data.TryGetUTF8EncodedBytes (out bytes)) {
        var msg = "It could not be UTF-8-encoded.";
        throw new ArgumentException (msg, "data");
      }

      if (bytes.LongLength <= WebSocket.FragmentLength)
        broadcastAsync (Opcode.Text, bytes, completed);
      else
        broadcastAsync (Opcode.Text, new MemoryStream (bytes), completed);
    }

    /// <summary>
    /// Sends the data from <paramref name="stream"/> asynchronously to
    /// every client in the WebSocket service.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   The data is sent as the binary data.
    ///   </para>
    ///   <para>
    ///   This method does not wait for the send to be complete.
    ///   </para>
    /// </remarks>
    /// <param name="stream">
    /// A <see cref="Stream"/> instance from which to read the data to send.
    /// </param>
    /// <param name="length">
    /// An <see cref="int"/> that specifies the number of bytes to send.
    /// </param>
    /// <param name="completed">
    ///   <para>
    ///   An <see cref="Action"/> delegate or <see langword="null"/>
    ///   if not needed.
    ///   </para>
    ///   <para>
    ///   The delegate invokes the method called when the send is complete.
    ///   </para>
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the manager is not Start.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="stream"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="stream"/> cannot be read.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="length"/> is less than 1.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   No data could be read from <paramref name="stream"/>.
    ///   </para>
    /// </exception>
    public void BroadcastAsync (Stream stream, int length, Action completed)
    {
      if (_state != ServerState.Start) {
        var msg = "The current state of the manager is not Start.";
        throw new InvalidOperationException (msg);
      }

      if (stream == null)
        throw new ArgumentNullException ("stream");

      if (!stream.CanRead) {
        var msg = "It cannot be read.";
        throw new ArgumentException (msg, "stream");
      }

      if (length < 1) {
        var msg = "Less than 1.";
        throw new ArgumentException (msg, "length");
      }

      var bytes = stream.ReadBytes (length);

      var len = bytes.Length;
      if (len == 0) {
        var msg = "No data could be read from it.";
        throw new ArgumentException (msg, "stream");
      }

      if (len < length) {
        _logger.Warn (
          String.Format (
            "Only {0} byte(s) of data could be read from the stream.",
            len
          )
        );
      }

      if (len <= WebSocket.FragmentLength)
        broadcastAsync (Opcode.Binary, bytes, completed);
      else
        broadcastAsync (Opcode.Binary, new MemoryStream (bytes), completed);
    }

    /// <summary>
    /// Sends a ping to every client in the WebSocket service.
    /// </summary>
    /// <returns>
    ///   <para>
    ///   A <c>Dictionary&lt;string, bool&gt;</c>.
    ///   </para>
    ///   <para>
    ///   It represents a collection of pairs of a session ID and
    ///   a value indicating whether a pong has been received from
    ///   the client within a time.
    ///   </para>
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The current state of the manager is not Start.
    /// </exception>
    [Obsolete ("This method will be removed.")]
    public Dictionary<string, bool> Broadping ()
    {
      if (_state != ServerState.Start) {
        var msg = "The current state of the manager is not Start.";
        throw new InvalidOperationException (msg);
      }

      return Broadping (WebSocketFrame.EmptyPingBytes, _waitTime);
    }

    /// <summary>
    /// Sends a ping with <paramref name="message"/> to every client in
    /// the WebSocket service.
    /// </summary>
    /// <returns>
    ///   <para>
    ///   A <c>Dictionary&lt;string, bool&gt;</c>.
    ///   </para>
    ///   <para>
    ///   It represents a collection of pairs of a session ID and
    ///   a value indicating whether a pong has been received from
    ///   the client within a time.
    ///   </para>
    /// </returns>
    /// <param name="message">
    ///   <para>
    ///   A <see cref="string"/> that represents the message to send.
    ///   </para>
    ///   <para>
    ///   The size must be 125 bytes or less in UTF-8.
    ///   </para>
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the manager is not Start.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="message"/> could not be UTF-8-encoded.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The size of <paramref name="message"/> is greater than 125 bytes.
    /// </exception>
    [Obsolete ("This method will be removed.")]
    public Dictionary<string, bool> Broadping (string message)
    {
      if (_state != ServerState.Start) {
        var msg = "The current state of the manager is not Start.";
        throw new InvalidOperationException (msg);
      }

      if (message.IsNullOrEmpty ())
        return Broadping (WebSocketFrame.EmptyPingBytes, _waitTime);

      byte[] bytes;
      if (!message.TryGetUTF8EncodedBytes (out bytes)) {
        var msg = "It could not be UTF-8-encoded.";
        throw new ArgumentException (msg, "message");
      }

      if (bytes.Length > 125) {
        var msg = "Its size is greater than 125 bytes.";
        throw new ArgumentOutOfRangeException ("message", msg);
      }

      var frame = WebSocketFrame.CreatePingFrame (bytes, false);
      return Broadping (frame.ToArray (), _waitTime);
    }

    /// <summary>
    /// Closes the specified session.
    /// </summary>
    /// <param name="id">
    /// A <see cref="string"/> that represents the ID of the session to close.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="id"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="id"/> is an empty string.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   The session could not be found.
    ///   </para>
    /// </exception>
    public void CloseSession (string id)
    {
      IWebSocketSession session;
      if (!TryGetSession (id, out session)) {
        var msg = "The session could not be found.";
        throw new ArgumentException (msg, "id");
      }

      session.Context.WebSocket.Close ();
    }

    /// <summary>
    /// Closes the session with the specified <paramref name="id"/>, <paramref name="code"/>,
    /// and <paramref name="reason"/>.
    /// </summary>
    /// <param name="id">
    /// A <see cref="string"/> that represents the ID of the session to close.
    /// </param>
    /// <param name="code">
    /// A <see cref="ushort"/> that represents the status code indicating the reason for the close.
    /// </param>
    /// <param name="reason">
    /// A <see cref="string"/> that represents the reason for the close.
    /// </param>
    public void CloseSession (string id, ushort code, string reason)
    {
      IWebSocketSession session;
      if (TryGetSession (id, out session))
        session.Context.WebSocket.Close (code, reason);
    }

    /// <summary>
    /// Closes the session with the specified <paramref name="id"/>, <paramref name="code"/>,
    /// and <paramref name="reason"/>.
    /// </summary>
    /// <param name="id">
    /// A <see cref="string"/> that represents the ID of the session to close.
    /// </param>
    /// <param name="code">
    /// One of the <see cref="CloseStatusCode"/> enum values, represents the status code
    /// indicating the reason for the close.
    /// </param>
    /// <param name="reason">
    /// A <see cref="string"/> that represents the reason for the close.
    /// </param>
    public void CloseSession (string id, CloseStatusCode code, string reason)
    {
      IWebSocketSession session;
      if (TryGetSession (id, out session))
        session.Context.WebSocket.Close (code, reason);
    }

    /// <summary>
    /// Sends a ping to the client using the specified session.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the send has done with no error and a pong has been
    /// received from the client within a time; otherwise, <c>false</c>.
    /// </returns>
    /// <param name="id">
    /// A <see cref="string"/> that represents the ID of the session.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="id"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="id"/> is an empty string.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   The session could not be found.
    ///   </para>
    /// </exception>
    public bool PingTo (string id)
    {
      IWebSocketSession session;
      if (!TryGetSession (id, out session)) {
        var msg = "The session could not be found.";
        throw new ArgumentException (msg, "id");
      }

      return session.Context.WebSocket.Ping ();
    }

    /// <summary>
    /// Sends a ping with <paramref name="message"/> to the client using
    /// the specified session.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the send has done with no error and a pong has been
    /// received from the client within a time; otherwise, <c>false</c>.
    /// </returns>
    /// <param name="message">
    ///   <para>
    ///   A <see cref="string"/> that represents the message to send.
    ///   </para>
    ///   <para>
    ///   The size must be 125 bytes or less in UTF-8.
    ///   </para>
    /// </param>
    /// <param name="id">
    /// A <see cref="string"/> that represents the ID of the session.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="id"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="id"/> is an empty string.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   The session could not be found.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="message"/> could not be UTF-8-encoded.
    ///   </para>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The size of <paramref name="message"/> is greater than 125 bytes.
    /// </exception>
    public bool PingTo (string message, string id)
    {
      IWebSocketSession session;
      if (!TryGetSession (id, out session)) {
        var msg = "The session could not be found.";
        throw new ArgumentException (msg, "id");
      }

      return session.Context.WebSocket.Ping (message);
    }

    /// <summary>
    /// Sends <paramref name="data"/> to the client using the specified session.
    /// </summary>
    /// <param name="data">
    /// An array of <see cref="byte"/> that represents the binary data to send.
    /// </param>
    /// <param name="id">
    /// A <see cref="string"/> that represents the ID of the session.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <para>
    ///   <paramref name="id"/> is <see langword="null"/>.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="data"/> is <see langword="null"/>.
    ///   </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="id"/> is an empty string.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   The session could not be found.
    ///   </para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The current state of the WebSocket connection is not Open.
    /// </exception>
    public void SendTo (byte[] data, string id)
    {
      IWebSocketSession session;
      if (!TryGetSession (id, out session)) {
        var msg = "The session could not be found.";
        throw new ArgumentException (msg, "id");
      }

      session.Context.WebSocket.Send (data);
    }

    /// <summary>
    /// Sends <paramref name="data"/> to the client using the specified session.
    /// </summary>
    /// <param name="data">
    /// A <see cref="string"/> that represents the text data to send.
    /// </param>
    /// <param name="id">
    /// A <see cref="string"/> that represents the ID of the session.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <para>
    ///   <paramref name="id"/> is <see langword="null"/>.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="data"/> is <see langword="null"/>.
    ///   </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="id"/> is an empty string.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   The session could not be found.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="data"/> could not be UTF-8-encoded.
    ///   </para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The current state of the WebSocket connection is not Open.
    /// </exception>
    public void SendTo (string data, string id)
    {
      IWebSocketSession session;
      if (!TryGetSession (id, out session)) {
        var msg = "The session could not be found.";
        throw new ArgumentException (msg, "id");
      }

      session.Context.WebSocket.Send (data);
    }

    /// <summary>
    /// Sends the data from <paramref name="stream"/> to the client using
    /// the specified session.
    /// </summary>
    /// <remarks>
    /// The data is sent as the binary data.
    /// </remarks>
    /// <param name="stream">
    /// A <see cref="Stream"/> instance from which to read the data to send.
    /// </param>
    /// <param name="length">
    /// An <see cref="int"/> that specifies the number of bytes to send.
    /// </param>
    /// <param name="id">
    /// A <see cref="string"/> that represents the ID of the session.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <para>
    ///   <paramref name="id"/> is <see langword="null"/>.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="stream"/> is <see langword="null"/>.
    ///   </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="id"/> is an empty string.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   The session could not be found.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="stream"/> cannot be read.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="length"/> is less than 1.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   No data could be read from <paramref name="stream"/>.
    ///   </para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The current state of the WebSocket connection is not Open.
    /// </exception>
    public void SendTo (Stream stream, int length, string id)
    {
      IWebSocketSession session;
      if (!TryGetSession (id, out session)) {
        var msg = "The session could not be found.";
        throw new ArgumentException (msg, "id");
      }

      session.Context.WebSocket.Send (stream, length);
    }

    /// <summary>
    /// Sends <paramref name="data"/> asynchronously to the client using
    /// the specified session.
    /// </summary>
    /// <remarks>
    /// This method does not wait for the send to be complete.
    /// </remarks>
    /// <param name="data">
    /// An array of <see cref="byte"/> that represents the binary data to send.
    /// </param>
    /// <param name="id">
    /// A <see cref="string"/> that represents the ID of the session.
    /// </param>
    /// <param name="completed">
    ///   <para>
    ///   An <c>Action&lt;bool&gt;</c> delegate or <see langword="null"/>
    ///   if not needed.
    ///   </para>
    ///   <para>
    ///   The delegate invokes the method called when the send is complete.
    ///   </para>
    ///   <para>
    ///   <c>true</c> is passed to the method if the send has done with
    ///   no error; otherwise, <c>false</c>.
    ///   </para>
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <para>
    ///   <paramref name="id"/> is <see langword="null"/>.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="data"/> is <see langword="null"/>.
    ///   </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="id"/> is an empty string.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   The session could not be found.
    ///   </para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The current state of the WebSocket connection is not Open.
    /// </exception>
    public void SendToAsync (byte[] data, string id, Action<bool> completed)
    {
      IWebSocketSession session;
      if (!TryGetSession (id, out session)) {
        var msg = "The session could not be found.";
        throw new ArgumentException (msg, "id");
      }

      session.Context.WebSocket.SendAsync (data, completed);
    }

    /// <summary>
    /// Sends <paramref name="data"/> asynchronously to the client using
    /// the specified session.
    /// </summary>
    /// <remarks>
    /// This method does not wait for the send to be complete.
    /// </remarks>
    /// <param name="data">
    /// A <see cref="string"/> that represents the text data to send.
    /// </param>
    /// <param name="id">
    /// A <see cref="string"/> that represents the ID of the session.
    /// </param>
    /// <param name="completed">
    ///   <para>
    ///   An <c>Action&lt;bool&gt;</c> delegate or <see langword="null"/>
    ///   if not needed.
    ///   </para>
    ///   <para>
    ///   The delegate invokes the method called when the send is complete.
    ///   </para>
    ///   <para>
    ///   <c>true</c> is passed to the method if the send has done with
    ///   no error; otherwise, <c>false</c>.
    ///   </para>
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <para>
    ///   <paramref name="id"/> is <see langword="null"/>.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="data"/> is <see langword="null"/>.
    ///   </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="id"/> is an empty string.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   The session could not be found.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="data"/> could not be UTF-8-encoded.
    ///   </para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The current state of the WebSocket connection is not Open.
    /// </exception>
    public void SendToAsync (string data, string id, Action<bool> completed)
    {
      IWebSocketSession session;
      if (!TryGetSession (id, out session)) {
        var msg = "The session could not be found.";
        throw new ArgumentException (msg, "id");
      }

      session.Context.WebSocket.SendAsync (data, completed);
    }

    /// <summary>
    /// Sends the data from <paramref name="stream"/> asynchronously to
    /// the client using the specified session.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   The data is sent as the binary data.
    ///   </para>
    ///   <para>
    ///   This method does not wait for the send to be complete.
    ///   </para>
    /// </remarks>
    /// <param name="stream">
    /// A <see cref="Stream"/> instance from which to read the data to send.
    /// </param>
    /// <param name="length">
    /// An <see cref="int"/> that specifies the number of bytes to send.
    /// </param>
    /// <param name="id">
    /// A <see cref="string"/> that represents the ID of the session.
    /// </param>
    /// <param name="completed">
    ///   <para>
    ///   An <c>Action&lt;bool&gt;</c> delegate or <see langword="null"/>
    ///   if not needed.
    ///   </para>
    ///   <para>
    ///   The delegate invokes the method called when the send is complete.
    ///   </para>
    ///   <para>
    ///   <c>true</c> is passed to the method if the send has done with
    ///   no error; otherwise, <c>false</c>.
    ///   </para>
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   <para>
    ///   <paramref name="id"/> is <see langword="null"/>.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="stream"/> is <see langword="null"/>.
    ///   </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="id"/> is an empty string.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   The session could not be found.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="stream"/> cannot be read.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="length"/> is less than 1.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   No data could be read from <paramref name="stream"/>.
    ///   </para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The current state of the WebSocket connection is not Open.
    /// </exception>
    public void SendToAsync (
      Stream stream, int length, string id, Action<bool> completed
    )
    {
      IWebSocketSession session;
      if (!TryGetSession (id, out session)) {
        var msg = "The session could not be found.";
        throw new ArgumentException (msg, "id");
      }

      session.Context.WebSocket.SendAsync (stream, length, completed);
    }

    /// <summary>
    /// Cleans up the inactive sessions in the WebSocket service.
    /// </summary>
    public void Sweep ()
    {
      if (_state != ServerState.Start || _sweeping || Count == 0)
        return;

      lock (_forSweep) {
        _sweeping = true;
        foreach (var id in InactiveIDs) {
          if (_state != ServerState.Start)
            break;

          lock (_sync) {
            IWebSocketSession session;
            if (_sessions.TryGetValue (id, out session)) {
              var state = session.State;
              if (state == WebSocketState.Open)
                session.Context.WebSocket.Close (CloseStatusCode.ProtocolError);
              else if (state == WebSocketState.Closing)
                continue;
              else
                _sessions.Remove (id);
            }
          }
        }

        _sweeping = false;
      }
    }

    /// <summary>
    /// Tries to get the session instance with the specified
    /// <paramref name="id"/>.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the session is successfully found;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <param name="id">
    /// A <see cref="string"/> that represents the ID of
    /// the session to find.
    /// </param>
    /// <param name="session">
    ///   <para>
    ///   When this method returns, a <see cref="IWebSocketSession"/>
    ///   instance or <see langword="null"/> if not found.
    ///   </para>
    ///   <para>
    ///   That session instance provides the function to
    ///   access the information in the session.
    ///   </para>
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="id"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="id"/> is an empty string.
    /// </exception>
    public bool TryGetSession (string id, out IWebSocketSession session)
    {
      if (id == null)
        throw new ArgumentNullException ("id");

      if (id.Length == 0)
        throw new ArgumentException ("An empty string.", "id");

      return tryGetSession (id, out session);
    }

    #endregion
  }
}
